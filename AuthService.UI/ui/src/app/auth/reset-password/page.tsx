'use client';

import { useSearchParams, useRouter } from 'next/navigation';
import { useActionState } from 'react';
import { resetPasswordAction, ResetPasswordState } from './action';
import { useEffect } from 'react';
import AuthLayout from '@/components/auth/AuthLayout';
import ResetPasswordCard from '@/components/auth/ResetPasswordCard';

const initialState: ResetPasswordState = {
  success: false,
};

export default function ResetPasswordPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token') || '';
  const [state, formAction, isPending] = useActionState(resetPasswordAction, initialState);

  useEffect(() => {
    if (state.success) {
      setTimeout(() => {
        router.push('/auth/login');
      }, 2000);
    }
  }, [state.success, router]);

  return (
    <AuthLayout>
      <ResetPasswordCard
        action={formAction}
        isPending={isPending}
        error={state.error}
        success={state.success}
        message={state.message}
        token={token}
      />
    </AuthLayout>
  );
}
