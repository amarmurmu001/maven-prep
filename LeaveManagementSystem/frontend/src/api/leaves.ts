import apiClient from './client';
import type { ApplyLeaveRequest, LeaveActionResponse, LeaveRequest, PagedResult } from '../types';

export const leavesApi = {
  getMyLeaves: async (page = 1, pageSize = 10): Promise<PagedResult<LeaveRequest>> => {
    const response = await apiClient.get(`/leaves/my-leaves?page=${page}&pageSize=${pageSize}`);
    return response.data;
  },

  apply: async (data: ApplyLeaveRequest): Promise<LeaveActionResponse> => {
    const response = await apiClient.post('/leaves/apply', data);
    return response.data;
  },

  cancel: async (id: string): Promise<LeaveActionResponse> => {
    const response = await apiClient.put(`/leaves/cancel/${id}`);
    return response.data;
  },
};
