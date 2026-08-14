import { z } from 'zod';

/**
 * Esquema estricto para la solicitud de préstamo.
 * - state: máximo 2 caracteres (ej: NY, CA)
 * - requestedAmount: se coerciona a número y debe ser > 0
 * - ssn: exactamente 9 dígitos
 */
export const loanApplicationSchema = z.object({
  firstName: z.string().min(1, 'El nombre es requerido'),
  lastName: z.string().min(1, 'El apellido es requerido'),
  address: z.string().min(1, 'La dirección es requerida'),
  state: z
    .string()
    .min(1, 'El estado es requerido')
    .max(2, 'El código del estado debe tener máximo 2 caracteres'),
  companyName: z.string().min(1, 'El nombre de la empresa es requerido'),
  requestedAmount: z.coerce
    .number({
      invalid_type_error: 'El monto solicitado debe ser un número',
    })
    .positive('El monto debe ser mayor a 0'),
  ssn: z
    .string()
    .regex(/^\d{9}$/, 'El SSN debe tener exactamente 9 dígitos'),
}).strict();

export type LoanApplication = z.infer<typeof loanApplicationSchema>;
