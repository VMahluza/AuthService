'use server'

import { post } from '@/lib/client';

export interface ForgotPasswordState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function forgotPasswordAction(prevState: ForgotPasswordState, formData: FormData): Promise<ForgotPasswordState> {
  const email = formData.get('email');

  const result = await post<Record<string, unknown>>('/auth/forgot-password', {
    email
  });

  if (!result.success) {
    return { error: result.error || 'Failed to send reset link', success: false };
  }

  return { 
    message: 'Password reset link sent! Please check your email.', 
    success: true, 
    data: result.data || {} 
  };
}
