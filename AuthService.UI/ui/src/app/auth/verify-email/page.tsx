'use client';

import { useSearchParams, useRouter } from 'next/navigation';
import { useEffect, useState, Suspense } from 'react';
import { verifyEmailAction } from './action';
import VerifyEmailCard from '@/components/auth/VerifyEmailCard';

function VerifyEmailContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token');
  const [response, setResponse] = useState(token ? 'Verifying...' : 'No verification token provided.');
  const [isSuccess, setIsSuccess] = useState(false);

  useEffect(() => {
    if (!token) {
      return;
    }

    const verifyEmail = async () => {
      try {
        const result = await verifyEmailAction(token);
        
        if (result.success) {
          setIsSuccess(true);
          setResponse(result.message || 'Email verified successfully!');
          setTimeout(() => {
            router.push('/auth/login');
          }, 2000);
        } else {
          setResponse(result.error || 'Failed to verify email');
        }
      } catch (error: unknown) {
        setResponse('Error: ' + (error instanceof Error ? error.message : 'Unknown error'));
      }
    };

    verifyEmail();
  }, [token, router]);

  return (
    <VerifyEmailCard message={response} isSuccess={isSuccess} />
  );
}

export default function VerifyEmailPage() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <VerifyEmailContent />
    </Suspense>
  );
}
