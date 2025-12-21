'use server'

import { post } from '@/lib/client/api-client'

export type RegisterState = {
  message?: string
  error?: string
  success?: boolean
}

export async function registerAction(prevState: RegisterState, formData: FormData): Promise<RegisterState> {
  const userName = formData.get('userName')
  const email = formData.get('email')
  const password = formData.get('password')

  const result = await post('/auth/register', {
    userName,
    email,
    password,
  })

  if (!result.success) {
    return { error: result.error || 'Registration failed' }
  }

  return { success: true, message: 'Registration successful! Please check your email to verify your account.' }
}
