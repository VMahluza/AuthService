'use server'

import { deleteSession } from '@/lib/session'
import { redirect } from 'next/navigation'
import { cookies } from 'next/headers'
import { del } from '@/lib/client'

export async function logoutAction(formData: FormData) {
  const revokeAllSessions = formData.get('revokeAllSessions') === 'on'
  const cookieStore = await cookies()
  const token = cookieStore.get('session')?.value

  if (token) {
    await del('/auth/logout', token, {
      jwtToken: token,
      revokeAllSessions: revokeAllSessions
    })
  }

  await deleteSession()
  redirect('/')
}
