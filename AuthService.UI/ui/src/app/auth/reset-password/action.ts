'use server'

import axios from 'axios';
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
    const response = await axios.post(`${BACKEND_BASE_URL}/auth/reset-password`, {
      token,
      newPassword
    });

    return { 
      message: 'Password reset successful! You can now login with your new password.', 
      success: true, 
      data: response.data 
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Failed to reset password';
      return { error: errorMessage, success: false };
    }
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
