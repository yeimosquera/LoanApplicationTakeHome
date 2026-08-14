'use client';

import React from 'react';
import { useRouter } from 'next/navigation';

export default function ApprovedPage() {
  const router = useRouter();

  return (
    <main className="min-h-screen bg-gray-50 flex items-center justify-center p-6">
      <div className="max-w-lg w-full">
        <div className="bg-white shadow-md rounded-lg p-8 text-center">
          <h1 className="text-2xl font-semibold text-green-700 mb-4">
            ¡Felicidades!
          </h1>
          <p className="text-gray-700 mb-6">
            Tu solicitud ha sido aprobada y procesada exitosamente.
          </p>

          <button
            onClick={() => router.push('/')}
            className="inline-flex items-center px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
          >
            Volver al inicio
          </button>
        </div>
      </div>
    </main>
  );
}
