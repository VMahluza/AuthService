'use server'

import { BACKEND_BASE_URL } from "@/lib/constants";

export interface ResetPasswordState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function resetPasswordAction(prevState: ResetPasswordState, formData: FormData): Promise<ResetPasswordState> {
  const token = formData.get('token');
  const newPassword = formData.get('newPassword');

  try {
    const response = await fetch(`${BACKEND_BASE_URL}/auth/reset-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token, newPassword }),
    });

    const text = await response.text();
    const result = text ? JSON.parse(text) : {};

    if (!response.ok) {
        return { error: result.detail || result.title || 'Failed to reset password', success: false };
    }

    return { message: 'Password reset successful! You can now login with your new password.', success: true, data: result };
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
