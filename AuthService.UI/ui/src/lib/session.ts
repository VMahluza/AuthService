import 'server-only'
import { cookies } from 'next/headers'

export async function createSession(accessToken: string, refreshToken: string) {
  const expiresAt = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000)
  const cookieStore = await cookies()
 
  cookieStore.set('session', accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    expires: expiresAt,
    sameSite: 'lax',
    path: '/',
  })

  // Optionally store refresh token if needed, or include it in the session object if we were encrypting a payload
  // For now, let's just store the access token as the primary session identifier
}

export async function deleteSession() {
  const cookieStore = await cookies()
  cookieStore.delete('session')
}
