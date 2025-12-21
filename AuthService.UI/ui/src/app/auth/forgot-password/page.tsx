'use client';

import { forgotPasswordAction, ForgotPasswordState } from './action';
import { useActionState } from 'react';
import AuthLayout from '@/components/auth/AuthLayout';
import ForgotPasswordCard from '@/components/auth/ForgotPasswordCard';

const initialState: ForgotPasswordState = {
  message: '',
  error: '',
  success: false,
  data: undefined
};

export default function ForgotPasswordPage() {
  const [state, formAction, isPending] = useActionState(forgotPasswordAction, initialState);

  return (
    <AuthLayout>
      <ForgotPasswordCard
        action={formAction}
        isPending={isPending}
        error={state.error}
        success={state.success}
        message={state.message}
      />
    </AuthLayout>
  );
}
