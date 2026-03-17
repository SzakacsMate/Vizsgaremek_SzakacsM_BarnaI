import { apiFetch, clearAccessToken, setAccessToken } from "./api";

type LoginRequest = {
  name: string;
  gmail: string;
  password: string;
};

type RegisterRequest = {
  name: string;
  gmail: string;
  password: string;
};

type LoginResponse = {
  accesToken: string;
  refreshToken: string;
  accessTokenExpiryTime: string;
  refreshTokenExpiryTime: string;
};

export type CurrentUserResponse = {
  id: string;
  name: string;
  role: string;
  imageUrl?: string;
  rep?: number;
};

export async function login(data: LoginRequest) {
  const result = await apiFetch<LoginResponse>("/login", {
    method: "POST",
    body: JSON.stringify({
      name: data.name,
      gmail: data.gmail,
      password: data.password,
    }),
  });

  setAccessToken(result.accesToken);
  return result;
}

export async function register(data: RegisterRequest) {
  return apiFetch("/register", {
    method: "POST",
    body: JSON.stringify({
      name: data.name,
      gmail: data.gmail,
      password: data.password,
    }),
  });
}

export async function getCurrentUser() {
  return apiFetch<CurrentUserResponse>("/CurrentUser");
}

export function logout() {
  clearAccessToken();
}