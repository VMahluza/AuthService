'use server';

import { get, type ApiResponse } from '@/lib/api-client';

interface Permission {
  id: string;
  name: string;
  description: string;
  resource?: string;
  action?: string;
}

export async function getPermissionsAction(token: string): Promise<ApiResponse<Permission[]>> {
  return get<Permission[]>('/permissions', token);
}
