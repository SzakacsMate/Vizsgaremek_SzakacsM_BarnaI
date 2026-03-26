import { apiFetch } from "./api";

export type ApiLobby = {
  id: string;
  lobbyName: string;
  dm: string;
  locationName: string;
  ttType: string;
  startDate: string;
  endDate: string;
  playerLimit: number;
  playerCount: number;
  playerMin: number;
  adress?: string;
  users?: string[];
  description?: string;
};

export async function getAllLobbies() {
  return apiFetch<ApiLobby[]>("/GetAllLobbies");
}

export async function getMyLobbies() {
  return apiFetch<ApiLobby[]>("/GetLobbies_youre_in");
}

export async function joinLobby(id: string) {
  return apiFetch(`/AddPlayer?Id=${id}`, {
    method: "POST",
  });
}

export async function leaveLobby(id: string) {
  return apiFetch(`/LeaveLobby?Id=${id}`, {
    method: "DELETE",
  });
}

export async function createLobby(
  locationId: string,
  data: {
    lobbyName: string;
    ttType: string;
    startDate: string;
    endDate: string;
    playerLimit: number;
    playerMin: number;
    description?: string;
  }
) {
  return apiFetch(`/CreateLobby?Id=${locationId}`, {
    method: "POST",
    body: JSON.stringify({
      lobbyName: data.lobbyName,
      ttType: data.ttType,
      startDate: data.startDate,
      endDate: data.endDate,
      playerLimit: data.playerLimit,
      playerMin: data.playerMin,
      description: data.description,
    }),
  });
}