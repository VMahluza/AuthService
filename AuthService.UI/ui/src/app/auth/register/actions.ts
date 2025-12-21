'use server'

import axios from 'axios'
import { BACKEND_BASE_URL } from '@/lib/constants'

export type RegisterState = {
  message?: string
  error?: string
  success?: boolean
}

export async function registerAction(prevState: RegisterState, formData: FormData): Promise<RegisterState> {
  const userName = formData.get('userName')
  const email = formData.get('email')
  const password = formData.get('password')

  if (!BACKEND_BASE_URL) {
    return { error: 'API URL is not configured' }
  }

  try {
    await axios.post(`${BACKEND_BASE_URL}/auth/register`, {
      userName,
      email,
      password,
    })

    return { success: true, message: 'Registration successful! Please check your email to verify your account.' }
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Registration failed'
      return { error: errorMessage }
    }
    const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
    return { error: errorMessage }
  }
}
