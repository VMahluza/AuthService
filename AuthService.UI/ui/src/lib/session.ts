import 'server-only'
import { cookies } from 'next/headers'

export async function createSession(accessToken: string, refreshToken: string) {
  const expiresAt = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000)
  const refreshExpiresAt = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000) // 30 days for refresh token
  const cookieStore = await cookies()
 
  // Store access token
  cookieStore.set('session', accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    expires: expiresAt,
    sameSite: 'lax',
    path: '/',
  })

  // Store refresh token separately with longer expiration
  cookieStore.set('refreshToken', refreshToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    expires: refreshExpiresAt,
    sameSite: 'lax',
    path: '/',
  })
}

export async function deleteSession() {
  const cookieStore = await cookies()
  cookieStore.delete('session')
  cookieStore.delete('refreshToken')
}

export async function getSession() {
  const cookieStore = await cookies()
  return cookieStore.get('session')?.value
}

export async function getRefreshToken() {
  const cookieStore = await cookies()
  return cookieStore.get('refreshToken')?.value
}
