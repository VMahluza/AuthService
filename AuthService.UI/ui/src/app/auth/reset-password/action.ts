'use server'

import { post } from '@/lib/client/api-client';

export interface ResetPasswordState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function resetPasswordAction(prevState: ResetPasswordState, formData: FormData): Promise<ResetPasswordState> {
  const token = formData.get('token');
  const newPassword = formData.get('newPassword');

  const result = await post<Record<string, unknown>>('/auth/reset-password', {
    token,
    newPassword
  });

  if (!result.success) {
    return { error: result.error || 'Failed to reset password', success: false };
  }

  return { 
    message: 'Password reset successful! You can now login with your new password.', 
    success: true, 
    data: result.data 
  };
}
