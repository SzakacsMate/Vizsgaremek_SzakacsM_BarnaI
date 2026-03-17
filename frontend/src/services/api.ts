const BASE_URL = "https://localhost:7231/api/Auth";

export function getAccessToken() {
  return localStorage.getItem("accessToken");
}

export function setAccessToken(token: string) {
  localStorage.setItem("accessToken", token);
}

export function clearAccessToken() {
  localStorage.removeItem("accessToken");
}

export async function apiFetch<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getAccessToken();

  const response = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers ?? {}),
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Request failed");
  }

  return response.json();
}