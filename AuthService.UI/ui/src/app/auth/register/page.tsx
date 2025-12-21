'use client';

import { useActionState } from 'react';
import { registerAction, RegisterState } from './actions';
import AuthLayout from '@/components/auth/AuthLayout';
import SignUpCard from '@/components/auth/SignUpCard';

const initialState: RegisterState = {
  message: '',
  error: '',
  success: false,
};

export default function RegisterPage() {
  const [state, formAction, isPending] = useActionState(registerAction, initialState);

  return (
    <AuthLayout>
      <SignUpCard
        action={formAction}
        isPending={isPending}
        error={state.error}
        success={state.success}
        message={state.message}
      />
    </AuthLayout>
  );
}
