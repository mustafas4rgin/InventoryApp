'use client';

import { useEffect, useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';

export default function ProfilePage() {
  const [user, setUser] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    axiosInstance.get('/auth/me')
      .then(res => setUser(res.data))
      .catch(err => console.error('Kullanıcı bilgileri alınamadı:', err))
      .finally(() => setLoading(false));
  }, []);

  const handlePasswordReset = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (newPassword !== confirmPassword) {
      setError('Yeni şifreler eşleşmiyor.');
      return;
    }

    try {
      const res = await axiosInstance.put('/auth/reset-password', {
        oldPassword,
        newPassword
      });

      const response = res.data;
      if (typeof response === 'object' && 'success' in response) {
        if (response.success) {
          setSuccess(response.message || 'Şifre başarıyla değiştirildi.');
          setIsModalOpen(false);
        } else {
          setError(response.message || 'Şifre değiştirilemedi.');
        }
      } else {
        setSuccess(response);
        setIsModalOpen(false);
      }
    } catch (err: any) {
      const msg = err.response?.data || 'Şifre güncelleme başarısız oldu';
      setError(msg);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64 text-gray-500 dark:text-gray-400">
        Profil yükleniyor...
      </div>
    );
  }

  if (!user) {
    return (
      <div className="text-center text-red-500">Kullanıcı bilgileri alınamadı.</div>
    );
  }

  const { firstName, lastName, email, createdAt, roleName, supplierName, isApproved, id } = user.user;
  const supplierId = user.supplierId;

  return (
    <div className="max-w-2xl mx-auto p-6 bg-white dark:bg-gray-900 shadow-md rounded-xl">
      <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-4">👤 Profil Bilgileri</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-gray-700 dark:text-gray-300">
        <div>
          <p className="text-sm font-medium">Ad Soyad</p>
          <p>{firstName} {lastName}</p>
        </div>
        <div>
          <p className="text-sm font-medium">E-posta</p>
          <p>{email}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Rol</p>
          <p>{roleName}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Tedarikçi Adı</p>
          <p>{supplierName}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Onay Durumu</p>
          <p>{isApproved ? 'Onaylandı' : 'Bekliyor'}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Kayıt Tarihi</p>
          <p>{createdAt}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Kullanıcı ID</p>
          <p>{id}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Supplier ID</p>
          <p>{supplierId}</p>
        </div>
      </div>

      <div className="mt-6 text-right">
        <button
          onClick={() => setIsModalOpen(true)}
          className="px-4 py-2 text-sm rounded bg-blue-600 hover:bg-blue-700 text-white"
        >
          🔒 Şifremi Güncelle
        </button>
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="bg-white dark:bg-gray-800 p-6 rounded-xl w-full max-w-md">
            <h2 className="text-lg font-bold mb-4 text-gray-800 dark:text-white">Şifre Güncelle</h2>
            <form onSubmit={handlePasswordReset} className="space-y-4">
              <input
                type="password"
                placeholder="Eski Şifre"
                value={oldPassword}
                onChange={e => setOldPassword(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-700 dark:text-white"
                required
              />
              <input
                type="password"
                placeholder="Yeni Şifre"
                value={newPassword}
                onChange={e => setNewPassword(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-700 dark:text-white"
                required
              />
              <input
                type="password"
                placeholder="Yeni Şifre (Tekrar)"
                value={confirmPassword}
                onChange={e => setConfirmPassword(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-700 dark:text-white"
                required
              />
              {(success || error) && (
                <p className={`text-sm font-medium ${success ? 'text-green-500' : 'text-red-500'}`}>
                  {success || error}
                </p>
              )}
              <div className="flex justify-end gap-2">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="px-4 py-2 text-sm bg-gray-300 dark:bg-gray-600 text-black dark:text-white rounded"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 text-sm bg-blue-600 hover:bg-blue-700 text-white rounded"
                >
                  Güncelle
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
