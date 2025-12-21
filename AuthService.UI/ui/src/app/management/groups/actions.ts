'use server';

import { get, post, type ApiResponse } from '@/lib/client';

interface Group {
  id: string;
  name: string;
  description: string;
}

export async function getGroupsAction(token: string): Promise<ApiResponse<Group[]>> {
  return get<Group[]>('/groups', token);
}

export async function createGroupAction(
  token: string,
  name: string,
  description: string
): Promise<ApiResponse<Group>> {
  return post<Group>('/groups', { name, description }, token);
}
