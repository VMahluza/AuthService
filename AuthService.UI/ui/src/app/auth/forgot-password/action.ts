'use server'

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
    const response = await fetch(`${BACKEND_BASE_URL}/auth/forgot-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email }),
    });

    // Some APIs might return 204 No Content or empty body
    const text = await response.text();
    const result = text ? JSON.parse(text) : {};

    if (!response.ok) {
        return { error: result.detail || result.title || 'Failed to send reset link', success: false };
    }

    return { message: 'Password reset link sent! Please check your email.', success: true, data: result };
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
