import { apiFetch } from "./api";

export type SearchUserResponse = {
  id?: string;
  name: string;
  profileI?: string;
  profileIUrl?: string;
  rep: number;
  role?: string;
};

export type ProfileCommentResponse = {
  kommentSzoveg: string;
  kommentaloUserId: string;
  kommentaloName: string;
};

export async function searchUserByName(name: string) {
  return apiFetch<SearchUserResponse>(
    `/Search%20for%20user(Name)?name=${encodeURIComponent(name)}`
  );
}

export async function searchUserById(id: string) {
  return apiFetch<SearchUserResponse>(
    `/Search%20for%20user(Id)?id=${encodeURIComponent(id)}`
  );
}

export async function getUserComments(userId: string) {
  return apiFetch<ProfileCommentResponse[]>(
    `/FogadoComments/${encodeURIComponent(userId)}`
  );
}

export async function addOrRemoveRep(userId: string, rep: 1 | -1) {
  const endpoint = rep === 1 ? "AddRep" : "RemoveRep";
  return apiFetch<void>(`/${endpoint}?id=${encodeURIComponent(userId)}`, {
    method: "PATCH",
  });
}

export async function deleteComment(commentId: string) {
  return apiFetch<void>(`/DeleteComment/${encodeURIComponent(commentId)}`, {
    method: "DELETE",
  });
}

export async function writeComment(data: {
  fogado: string;
  kommentSzoveg: string;
}) {
  return apiFetch<void>(`/WriteComment?Id=${encodeURIComponent(data.fogado)}`, {
    method: "POST",
    body: JSON.stringify({ kommentSzoveg: data.kommentSzoveg }),
  });
}

export async function changeUserData(data: {
  currentName: string;
  currentPassword: string;
  changeName: string;
  changePassword: string;
  changeProfileI: string;
}) {
  return apiFetch<{
    accesToken: string;
    refreshToken: string;
    accessTokenExpiryTime: string;
    refreshTokenExpiryTime: string;
  }>("/ChangeUserData", {
    method: "PATCH",
    body: JSON.stringify(data),
  });
}