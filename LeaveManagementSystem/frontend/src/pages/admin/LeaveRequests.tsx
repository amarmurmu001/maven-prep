import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApi } from '../../api/admin';
import { formatDate, formatDateTime, getStatusColor } from '../../utils/format';

export default function AdminLeaveRequests() {
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['adminLeaves', page],
    queryFn: () => adminApi.getAllLeaves(page),
  });

  const approveMutation = useMutation({
    mutationFn: (id: string) => adminApi.approveLeave(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminLeaves'] });
      queryClient.invalidateQueries({ queryKey: ['adminDashboard'] });
      setToast({ message: 'Leave approved successfully', type: 'success' });
    },
    onError: () => setToast({ message: 'Failed to approve leave', type: 'error' }),
  });

  const rejectMutation = useMutation({
    mutationFn: (id: string) => adminApi.rejectLeave(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminLeaves'] });
      queryClient.invalidateQueries({ queryKey: ['adminDashboard'] });
      setToast({ message: 'Leave rejected', type: 'success' });
    },
    onError: () => setToast({ message: 'Failed to reject leave', type: 'error' }),
  });

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Leave Requests</h1>

      {toast && (
        <div className={`p-3 text-sm rounded-lg ${
          toast.type === 'success'
            ? 'bg-green-50 text-green-700 dark:bg-green-900/50 dark:text-green-200'
            : 'bg-red-50 text-red-700 dark:bg-red-900/50 dark:text-red-200'
        }`}>
          {toast.message}
          <button onClick={() => setToast(null)} className="float-right font-bold">&times;</button>
        </div>
      )}

      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
        {isLoading ? (
          <div className="p-6 text-center text-gray-500">Loading...</div>
        ) : !data?.items.length ? (
          <div className="p-6 text-center text-gray-500">No leave requests found</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 dark:bg-gray-700">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Employee</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Type</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Dates</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Days</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Applied</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
                {data.items.map((leave) => (
                  <tr key={leave.id} className="hover:bg-gray-50 dark:hover:bg-gray-700">
                    <td className="px-6 py-4 text-gray-900 dark:text-white font-medium">{leave.employeeName}</td>
                    <td className="px-6 py-4 text-gray-600 dark:text-gray-300">{leave.leaveType}</td>
                    <td className="px-6 py-4 text-gray-600 dark:text-gray-300">
                      {formatDate(leave.startDate)} - {formatDate(leave.endDate)}
                    </td>
                    <td className="px-6 py-4 text-gray-600 dark:text-gray-300">{leave.durationDays}</td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 text-xs font-medium rounded-full ${getStatusColor(leave.status)}`}>
                        {leave.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-gray-600 dark:text-gray-300">{formatDateTime(leave.createdAt)}</td>
                    <td className="px-6 py-4">
                      <div className="flex gap-2">
                        {leave.status === 'Pending' && (
                          <>
                            <button
                              onClick={() => approveMutation.mutate(leave.id)}
                              disabled={approveMutation.isPending}
                              className="px-3 py-1 text-xs font-medium text-green-600 bg-green-50 hover:bg-green-100 dark:bg-green-900/30 dark:text-green-400 rounded-lg disabled:opacity-50"
                            >
                              Approve
                            </button>
                            <button
                              onClick={() => rejectMutation.mutate(leave.id)}
                              disabled={rejectMutation.isPending}
                              className="px-3 py-1 text-xs font-medium text-red-600 bg-red-50 hover:bg-red-100 dark:bg-red-900/30 dark:text-red-400 rounded-lg disabled:opacity-50"
                            >
                              Reject
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {data && data.totalPages > 1 && (
          <div className="flex items-center justify-between px-6 py-4 border-t border-gray-200 dark:border-gray-700">
            <button
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={!data.hasPreviousPage}
              className="px-3 py-1 text-sm text-gray-600 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 rounded-lg disabled:opacity-50"
            >
              Previous
            </button>
            <span className="text-sm text-gray-600 dark:text-gray-300">
              Page {page} of {data.totalPages}
            </span>
            <button
              onClick={() => setPage((p) => p + 1)}
              disabled={!data.hasNextPage}
              className="px-3 py-1 text-sm text-gray-600 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 rounded-lg disabled:opacity-50"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
