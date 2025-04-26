import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Eğer kullanıcı login sayfasında ve zaten accessToken varsa dashboard'a yönlendir
  const accessToken = request.cookies.get("accessToken")?.value;

  if (pathname === "/login" && accessToken) {
    return NextResponse.redirect(new URL("/dashboard", request.url));
  }

  // Eğer kullanıcı korumalı bir sayfaya gidiyorsa ve accessToken yoksa login'e yönlendir
  if (pathname.startsWith("/dashboard") || pathname.startsWith("/products") || pathname.startsWith("/categories") || pathname.startsWith("/suppliers")) {
    if (!accessToken) {
      return NextResponse.redirect(new URL("/login", request.url));
    }
  }

  return NextResponse.next();
}

// Bu middleware hangi route'larda çalışacak
export const config = {
  matcher: [
    "/dashboard/:path*", 
    "/products/:path*", 
    "/categories/:path*", 
    "/suppliers/:path*", 
    "/login"
  ],
};
