'use server';

import { get, post, type ApiResponse } from '@/lib/client';

interface Role {
  id: string;
  name: string;
  description: string;
}

export async function getRolesAction(token: string): Promise<ApiResponse<Role[]>> {
  return get<Role[]>('/roles', token);
}

export async function createRoleAction(
  token: string,
  name: string,
  description: string
): Promise<ApiResponse<Role>> {
  return post<Role>('/roles', { name, description }, token);
}
