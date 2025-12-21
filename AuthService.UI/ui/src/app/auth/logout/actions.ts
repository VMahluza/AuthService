'use server'

import { deleteSession } from '@/lib/session'
import { redirect } from 'next/navigation'
import { cookies } from 'next/headers'
import axios from 'axios'
import { BACKEND_BASE_URL } from '@/lib/constants'

export async function logoutAction(formData: FormData) {
  const revokeAllSessions = formData.get('revokeAllSessions') === 'on'
  const cookieStore = await cookies()
  const token = cookieStore.get('session')?.value

  if (token && BACKEND_BASE_URL) {
    try {
      await axios.delete(`${BACKEND_BASE_URL}/auth/logout`, {
        data: {
          jwtToken: token,
          revokeAllSessions: revokeAllSessions
        }
      })
    } catch (error) {
      console.error('Logout failed on backend', error)
    }
  }

  await deleteSession()
  redirect('/')
}
