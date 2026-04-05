const BASE_URL = "https://localhost:7231/api/Auth";

let onAuthFailure: (() => void) | null = null;

export function setOnAuthFailure(callback: () => void) {
  onAuthFailure = callback;
}

export function getAccessToken() {
  return localStorage.getItem("accessToken");
}

export function setAccessToken(token: string) {
  localStorage.setItem("accessToken", token);
}

export function clearAccessToken() {
  localStorage.removeItem("accessToken");
}

export function getRefreshToken() {
  return localStorage.getItem("refreshToken");
}

export function setRefreshToken(token: string) {
  localStorage.setItem("refreshToken", token);
}

export function clearRefreshToken() {
  localStorage.removeItem("refreshToken");
}

export function setTokenExpiries(accessExpiry: string, refreshExpiry: string) {
  localStorage.setItem("accessTokenExpiry", accessExpiry);
  localStorage.setItem("refreshTokenExpiry", refreshExpiry);
}

export function clearTokenExpiries() {
  localStorage.removeItem("accessTokenExpiry");
  localStorage.removeItem("refreshTokenExpiry");
}

function isTokenExpired(expiryKey: string): boolean {
  const expiry = localStorage.getItem(expiryKey);
  if (!expiry) return true;
  return new Date(expiry).getTime() <= Date.now();
}

let refreshPromise: Promise<boolean> | null = null;

async function tryRefreshToken(): Promise<boolean> {
  if (refreshPromise) return refreshPromise;

  refreshPromise = (async () => {
    const refreshToken = getRefreshToken();
    const accessToken = getAccessToken();

    if (!refreshToken || isTokenExpired("refreshTokenExpiry")) {
      clearAccessToken();
      clearRefreshToken();
      clearTokenExpiries();
      onAuthFailure?.();
      return false;
    }

    try {
      const response = await fetch(`${BASE_URL}/Refresh`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          accesToken: accessToken,
          refreshToken: refreshToken,
        }),
      });

      if (!response.ok) {
        clearAccessToken();
        clearRefreshToken();
        clearTokenExpiries();
        onAuthFailure?.();
        return false;
      }

      const data = await response.json();
      setAccessToken(data.accesToken);
      setRefreshToken(data.refreshToken);
      setTokenExpiries(data.accessTokenExpiryTime, data.refreshTokenExpiryTime);
      return true;
    } catch {
      clearAccessToken();
      clearRefreshToken();
      clearTokenExpiries();
      onAuthFailure?.();
      return false;
    }
  })();

  try {
    return await refreshPromise;
  } finally {
    refreshPromise = null;
  }
}

export async function apiFetch<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  if (getAccessToken() && isTokenExpired("accessTokenExpiry")) {
    const refreshed = await tryRefreshToken();
    if (!refreshed) {
      throw new Error("Session expired");
    }
  }

  const token = getAccessToken();

  const response = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers ?? {}),
    },
  });

  if (response.status === 401 && getRefreshToken()) {
    const refreshed = await tryRefreshToken();
    if (refreshed) {
      const newToken = getAccessToken();
      const retryResponse = await fetch(`${BASE_URL}${endpoint}`, {
        ...options,
        headers: {
          "Content-Type": "application/json",
          ...(newToken ? { Authorization: `Bearer ${newToken}` } : {}),
          ...(options.headers ?? {}),
        },
      });

      if (!retryResponse.ok) {
        const errorText = await retryResponse.text();
        throw new Error(errorText || "Request failed");
      }

      const retryText = await retryResponse.text();
      return (retryText ? JSON.parse(retryText) : undefined) as T;
    } else {
      throw new Error("Session expired");
    }
  }

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || "Request failed");
  }

  const text = await response.text();
  return (text ? JSON.parse(text) : undefined) as T;
}