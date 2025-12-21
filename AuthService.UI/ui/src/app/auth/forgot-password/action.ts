'use server'

import axios from 'axios';
import { BACKEND_BASE_URL } from "@/lib/constants";

export interface ForgotPasswordState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function forgotPasswordAction(prevState: ForgotPasswordState, formData: FormData): Promise<ForgotPasswordState> {
  const email = formData.get('email');

  try {
    const response = await axios.post(`${BACKEND_BASE_URL}/auth/forgot-password`, {
      email
    });

    return { 
      message: 'Password reset link sent! Please check your email.', 
      success: true, 
      data: response.data || {} 
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Failed to send reset link';
      return { error: errorMessage, success: false };
    }
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
