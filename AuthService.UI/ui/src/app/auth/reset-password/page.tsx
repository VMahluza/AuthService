'use client';

import { useSearchParams, useRouter } from 'next/navigation';
import { useActionState, Suspense } from 'react';
import { resetPasswordAction, ResetPasswordState } from './action';
import { useEffect } from 'react';
import ResetPasswordCard from '@/components/auth/ResetPasswordCard';

const initialState: ResetPasswordState = {
  success: false,
};

function ResetPasswordContent() {
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
    <ResetPasswordCard
      action={formAction}
      isPending={isPending}
      error={state.error}
      success={state.success}
      message={state.message}
      token={token}
    />
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <ResetPasswordContent />
    </Suspense>
  );
}
