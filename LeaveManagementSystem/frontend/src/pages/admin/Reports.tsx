import { useQuery } from '@tanstack/react-query';
import { adminApi } from '../../api/admin';

export default function AdminReports() {
  const { data, isLoading } = useQuery({
    queryKey: ['adminDashboard'],
    queryFn: () => adminApi.getDashboard(),
  });

  if (isLoading) {
    return <div className="text-center text-gray-500 py-12">Loading reports...</div>;
  }

  if (!data) return null;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Reports</h1>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">Leave Summary</h2>
          <dl className="space-y-3">
            <div className="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
              <dt className="text-gray-600 dark:text-gray-300">Pending Requests</dt>
              <dd className="font-semibold text-yellow-600">{data.pendingRequests}</dd>
            </div>
            <div className="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
              <dt className="text-gray-600 dark:text-gray-300">Approved Requests</dt>
              <dd className="font-semibold text-green-600">{data.approvedRequests}</dd>
            </div>
            <div className="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
              <dt className="text-gray-600 dark:text-gray-300">Rejected Requests</dt>
              <dd className="font-semibold text-red-600">{data.rejectedRequests}</dd>
            </div>
            <div className="flex justify-between py-2">
              <dt className="text-gray-600 dark:text-gray-300">Total This Month</dt>
              <dd className="font-semibold text-blue-600">{data.totalLeavesThisMonth}</dd>
            </div>
          </dl>
        </div>

        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">Employee Overview</h2>
          <dl className="space-y-3">
            <div className="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
              <dt className="text-gray-600 dark:text-gray-300">Total Employees</dt>
              <dd className="font-semibold text-gray-900 dark:text-white">{data.totalEmployees}</dd>
            </div>
            <div className="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
              <dt className="text-gray-600 dark:text-gray-300">Active Employees</dt>
              <dd className="font-semibold text-green-600">{data.activeEmployees}</dd>
            </div>
            <div className="flex justify-between py-2">
              <dt className="text-gray-600 dark:text-gray-300">Inactive Employees</dt>
              <dd className="font-semibold text-red-600">{data.totalEmployees - data.activeEmployees}</dd>
            </div>
          </dl>
        </div>

        <div className="lg:col-span-2 bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">Leave Type Distribution</h2>
          <div className="space-y-3">
            {Object.entries(data.leaveTypeDistribution).map(([type, count]) => {
              const total = Object.values(data.leaveTypeDistribution).reduce((a, b) => a + b, 0);
              const percentage = total > 0 ? Math.round((count / total) * 100) : 0;

              return (
                <div key={type}>
                  <div className="flex justify-between text-sm mb-1">
                    <span className="text-gray-600 dark:text-gray-300">
                      {type.replace(/([A-Z])/g, ' $1').trim()}
                    </span>
                    <span className="text-gray-900 dark:text-white font-medium">{percentage}%</span>
                  </div>
                  <div className="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2.5">
                    <div
                      className="bg-blue-600 h-2.5 rounded-full"
                      style={{ width: `${percentage}%` }}
                    />
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
}
