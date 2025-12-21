import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

export function proxy(request: NextRequest) {
  const path = request.nextUrl.pathname
  const isProtectedRoute = path.startsWith('/management')
  const isPublicRoute = path.startsWith('/auth/login') || path.startsWith('/auth/register')

  const cookie = request.cookies.get('session')?.value
  
  if (isProtectedRoute && !cookie) {
    return NextResponse.redirect(new URL('/auth/login', request.nextUrl))
  }

  if (isPublicRoute && cookie) {
    return NextResponse.redirect(new URL('/management/dashboard', request.nextUrl))
  }

  return NextResponse.next()
}

export const config = {
  matcher: [
    '/management/:path*',
    '/auth/login',
    '/auth/register',
  ],
}
