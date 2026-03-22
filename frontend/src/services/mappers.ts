import type { Location } from "../types/location";
import type { Session } from "../types/session";
import type { User, UserComment } from "../types/user";
import type { CurrentUserResponse } from "./authService";
import type { ApiLocation } from "./locationService";
import type { ApiLobby } from "./lobbyService";
import type {
  ProfileCommentResponse,
  SearchUserResponse,
} from "./profileService";

const systemColorMap: Record<string, string> = {
  "D&D 5e": "#ff1f1f",
  "Pathfinder 2e": "#d6d800",
  "Call of Cthulhu 7e": "#00cc22",
};

export function mapApiLocationToLocation(apiLocation: ApiLocation): Location {
  return {
    id: apiLocation.id,
    name: apiLocation.locationName,
    address: apiLocation.adress,
    description: apiLocation.description,
    longDescription: apiLocation.description,
    image: apiLocation.image || "",
    bookedDates: [],
  };
}

export function mapApiLobbyToSession(apiLobby: ApiLobby): Session {
  const users = apiLobby.users ?? [];

  return {
    id: apiLobby.id,
    title:
      apiLobby.lobbyName && apiLobby.lobbyName.trim() !== ""
        ? apiLobby.lobbyName
        : `${apiLobby.ttType} Session`,
    system: apiLobby.ttType,
    sessionNumber: 1,
    date: new Date(apiLobby.startDate).toLocaleString(),
    dateKey: apiLobby.startDate,
    duration: `${Math.max(
      1,
      Math.round(
        (new Date(apiLobby.endDate).getTime() - new Date(apiLobby.startDate).getTime()) /
          3600000
      )
    )}h`,
    location: apiLobby.locationName,
    minPlayers: apiLobby.playerMin,
    playerLimit: apiLobby.playerLimit,
    players: users,
    systemColor: systemColorMap[apiLobby.ttType] ?? "#999999",
    dmName: apiLobby.dm,
    status:
      apiLobby.playerCount >= apiLobby.playerMin ? "confirmed" : "pending",
  };
}

export function mapApiCurrentUserToUser(apiUser: CurrentUserResponse): User {
  return {
    id: apiUser.id,
    name: apiUser.name,
    image: apiUser.imageUrl || "",
    rep: apiUser.rep ?? 0,
    role: apiUser.role,
    comments: [],
  };
}

export function mapSearchUserToUser(apiUser: SearchUserResponse, fallbackId: string): User {
  return {
    id: apiUser.id || fallbackId,
    name: apiUser.name,
    image: apiUser.profileI || apiUser.profileIUrl || "",
    rep: apiUser.rep ?? 0,
    role: apiUser.role,
    comments: [],
  };
}

export function mapProfileCommentsToUserComments(
  comments: ProfileCommentResponse[]
): UserComment[] {
  return comments.map((comment, index) => ({
    id: `${comment.kommentaloUserId}-${index}`,
    text: comment.kommentSzoveg,
    authorName: comment.name,
  }));
}