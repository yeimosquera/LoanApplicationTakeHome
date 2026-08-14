'use client';

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loanApplicationSchema, LoanApplication } from '../lib/loanSchema';

export default function HomePage() {
  const router = useRouter();
  const [submitting, setSubmitting] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoanApplication>({
    resolver: zodResolver(loanApplicationSchema),
    mode: 'onTouched',
  });

  const onSubmit = async (values: LoanApplication) => {
    setSubmitting(true);
    try {
      const res = await fetch('http://localhost:5000/api/applications', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(values),
      });

      if (!res.ok) {
        // Intentar leer mensaje del backend si lo provee
        let text = 'Error del servidor';
        try {
          const body = await res.json();
          text = body?.message ?? text;
        } catch {
          // ignore
        }
        throw new Error(text);
      }

      const data: { isApproved: boolean; denialReason: string | null } =
        await res.json();

      if (data.isApproved) {
        router.push('/approved');
      } else {
        const reason = data.denialReason ?? 'Sin motivo especificado';
        router.push(`/denied?reason=${encodeURIComponent(reason)}`);
      }
    } catch (err: any) {
      // Mostrar una alerta simple para errores de red/servidor
      alert(err?.message ?? 'Ocurrió un error al enviar la solicitud');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <main className="min-h-screen bg-gray-50 flex items-center justify-center p-6">
      <div className="max-w-xl w-full">
        <div className="bg-white shadow-md rounded-lg p-8">
          <h1 className="text-2xl font-semibold text-gray-800 mb-2">
            Solicitud de Préstamo
          </h1>
          <p className="text-sm text-gray-500 mb-6">
            Completa la información para procesar tu solicitud.
          </p>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Nombre
                </label>
                <input
                  type="text"
                  {...register('firstName')}
                  className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                    ${errors.firstName ? 'border-red-500' : ''}`}
                />
                {errors.firstName && (
                  <p className="mt-1 text-xs text-red-600">
                    {errors.firstName.message}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Apellido
                </label>
                <input
                  type="text"
                  {...register('lastName')}
                  className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                    ${errors.lastName ? 'border-red-500' : ''}`}
                />
                {errors.lastName && (
                  <p className="mt-1 text-xs text-red-600">
                    {errors.lastName.message}
                  </p>
                )}
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">
                Dirección
              </label>
              <input
                type="text"
                {...register('address')}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                  ${errors.address ? 'border-red-500' : ''}`}
              />
              {errors.address && (
                <p className="mt-1 text-xs text-red-600">
                  {errors.address.message}
                </p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Estado (ej: NY)
                </label>
                <input
                  type="text"
                  maxLength={2}
                  {...register('state')}
                  className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                    ${errors.state ? 'border-red-500' : ''}`}
                />
                {errors.state && (
                  <p className="mt-1 text-xs text-red-600">{errors.state.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Empresa
                </label>
                <input
                  type="text"
                  {...register('companyName')}
                  className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                    ${errors.companyName ? 'border-red-500' : ''}`}
                />
                {errors.companyName && (
                  <p className="mt-1 text-xs text-red-600">
                    {errors.companyName.message}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Monto Solicitado
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  {...register('requestedAmount')}
                  className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                    ${errors.requestedAmount ? 'border-red-500' : ''}`}
                />
                {errors.requestedAmount && (
                  <p className="mt-1 text-xs text-red-600">
                    {errors.requestedAmount.message}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  SSN (9 dígitos)
                </label>
                <input
                  type="text"
                  inputMode="numeric"
                  maxLength={9}
                  {...register('ssn')}
                  className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm
                    ${errors.ssn ? 'border-red-500' : ''}`}
                />
                {errors.ssn && (
                  <p className="mt-1 text-xs text-red-600">{errors.ssn.message}</p>
                )}
              </div>
            </div>

            <div className="pt-4">
              <button
                type="submit"
                disabled={submitting}
                className={`w-full inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white
                  ${submitting ? 'bg-indigo-300 cursor-not-allowed' : 'bg-indigo-600 hover:bg-indigo-700'}
                `}
              >
                {submitting ? (
                  <>
                    <svg
                      className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <circle
                        className="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        strokeWidth="4"
                      />
                      <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
                      />
                    </svg>
                    Enviando...
                  </>
                ) : (
                  'Enviar solicitud'
                )}
              </button>
            </div>
          </form>
        </div>

        <p className="text-xs text-gray-400 text-center mt-4">
          Datos de prueba solo para el frontend — el backend se encuentra en
          http://localhost:5000
        </p>
      </div>
    </main>
  );
}
