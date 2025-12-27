'use server';

import { get, post, type ApiResponse } from '@/lib/client/index';
import { getSession } from '@/lib/session';
import { revalidatePath } from 'next/cache';

interface Role {
  id: string;
  name: string;
  description: string;
  createdAt: string;
  lastUpdatedAt?: string;
}

interface RolesListResponse {
  roles: Role[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export async function getRolesAction(): Promise<ApiResponse<Role[]>> {
  try {
    const token = await getSession();
    if (!token) {
      return { success: false, error: 'Unauthorized' };
    }
    
    console.log('Fetching roles with token:', token);
    const result = await get<RolesListResponse>('/roles', token);
    
    if (!result.success) {
      console.error('Failed to fetch roles:', result.error);
      return { 
        success: false, 
        error: result.error || 'Failed to fetch roles' 
      };
    }
    
    // Extract roles array from paginated response
    if (result.data && result.data.roles) {
      return {
        success: true,
        data: result.data.roles
      };
    }
    
    return {
      success: true,
      data: []
    };
  } catch (error) {
    console.error('Error in getRolesAction:', error);
    return { 
      success: false, 
      error: error instanceof Error ? error.message : 'An unexpected error occurred while fetching roles' 
    };
  }
}

export async function createRoleAction(
  prevState: ApiResponse<Role> | undefined,
  formData: FormData
): Promise<ApiResponse<Role>> {
  const token = await getSession();
  if (!token) {
    return { success: false, error: 'Unauthorized' };
  }

  const name = formData.get('name') as string;
  const description = formData.get('description') as string;

  if (!name) {
    return { success: false, error: 'Role name is required' };
  }

  const result = await post<Role>('/roles', { name, description }, token);

  if (result.success) {
    revalidatePath('/management/roles');
  }

  return result;
}
