'use client';

import { destroyCookie } from 'nookies';
import { useRouter } from 'next/navigation';

export default function useLogout() {
  const router = useRouter();

  const logout = () => {
    destroyCookie(null, 'accessToken');
    destroyCookie(null, 'refreshToken');

    router.push('/login');
  };

  return logout;
}
