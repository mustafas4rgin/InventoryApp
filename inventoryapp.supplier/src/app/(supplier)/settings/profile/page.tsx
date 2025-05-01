'use client';

import { useEffect, useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';
import toast from 'react-hot-toast';

export default function ProfileSettings() {
  const [user, setUser] = useState({ id: 0, firstName: '', lastName: '', email: '', roleId: 0, supplierId: 0 });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    axiosInstance.get('/auth/me')
      .then(res => {
        setUser({
          id: res.data.user.id,
          firstName: res.data.user.firstName,
          lastName: res.data.user.lastName,
          email: res.data.user.email,
          roleId: res.data.user.roleId,
          supplierId: res.data.user.supplierId,
        });
      });
  }, []);

  const extractErrorMessage = (err: any) => {
    if (Array.isArray(err.response?.data?.errors)) {
      return err.response.data.errors.map((e: any) => e.errorMessage).join(' | ');
    }
    if (typeof err.response?.data?.message === 'string') {
      return err.response.data.message;
    }
    return 'İşlem başarısız oldu';
  };

  const handleProfileSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setSaving(true);
      const payload = {
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        roleId: user.roleId,
        supplierId: user.supplierId,
      };
      const res = await axiosInstance.put(`/user/update/${user.id}`, payload);
      const message = res.data?.message || 'Profil güncellendi';
      toast.success(message);
    } catch (err: any) {
      toast.error(extractErrorMessage(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-[#0f172a] flex items-center justify-center px-4">
      <div className="w-full max-w-lg bg-white dark:bg-gray-900 rounded-2xl shadow-xl p-8">
        <h1 className="text-3xl font-bold text-center text-gray-900 dark:text-white mb-6">👤 Profil Bilgileri</h1>
        <form onSubmit={handleProfileSave} className="space-y-5">
          <input
            type="text"
            value={user.firstName}
            onChange={(e) => setUser({ ...user, firstName: e.target.value })}
            placeholder="Adınız"
            required
            className="w-full px-4 py-2 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white shadow-sm"
          />
          <input
            type="text"
            value={user.lastName}
            onChange={(e) => setUser({ ...user, lastName: e.target.value })}
            placeholder="Soyadınız"
            required
            className="w-full px-4 py-2 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white shadow-sm"
          />
          <input
            type="email"
            value={user.email}
            onChange={(e) => setUser({ ...user, email: e.target.value })}
            placeholder="Email"
            required
            className="w-full px-4 py-2 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white shadow-sm"
          />
          <button
            type="submit"
            disabled={saving}
            className="w-full py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition shadow-md"
          >
            {saving ? 'Kaydediliyor...' : 'Profili Kaydet'}
          </button>
        </form>
      </div>
    </div>
  );
}
