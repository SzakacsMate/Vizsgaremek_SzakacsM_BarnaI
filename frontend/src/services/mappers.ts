import randomPic1 from "/public/assets/profilepics/random/RandomProfilePic1.png";
import randomPic2 from "/public/assets/profilepics/random/RandomProfilePic2.png";
import randomPic3 from "/public/assets/profilepics/random/RandomProfilePic3.png";
import randomPic4 from "/public/assets/profilepics/random/RandomProfilePic4.png";
import randomPic5 from "/public/assets/profilepics/random/RandomProfilePic5.png";
import randomPic6 from "/public/assets/profilepics/random/RandomProfilePic6.png";
import randomPic7 from "/public/assets/profilepics/random/RandomProfilePic7.png";
import randomPic8 from "/public/assets/profilepics/random/RandomProfilePic8.png";
import randomPic9 from "/public/assets/profilepics/random/RandomProfilePic9.png";
import randomPic10 from "/public/assets/profilepics/random/RandomProfilePic10.png";
import randomPic11 from "/public/assets/profilepics/random/RandomProfilePic11.png";
import randomPic12 from "/public/assets/profilepics/random/RandomProfilePic12.png";
import randomPic13 from "/public/assets/profilepics/random/RandomProfilePic13.png";
import randomPic14 from "/public/assets/profilepics/random/RandomProfilePic14.png";
import randomPic15 from "/public/assets/profilepics/random/RandomProfilePic15.png";

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

const fallbackProfilePics = [
  randomPic1,
  randomPic2,
  randomPic3,
  randomPic4,
  randomPic5,
  randomPic6,
  randomPic7,
  randomPic8,
  randomPic9,
  randomPic10,
  randomPic11,
  randomPic12,
  randomPic13,
  randomPic14,
  randomPic15,
];

function getFallbackProfilePic(name: string) {
  let sum = 0;
  for (let i = 0; i < name.length; i += 1) {
    sum += name.charCodeAt(i);
  }
  return fallbackProfilePics[sum % fallbackProfilePics.length];
}

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

  const start = new Date(apiLobby.startDate);
  const end = apiLobby.endDate ? new Date(apiLobby.endDate) : null;

  const durationHours =
    end && !Number.isNaN(end.getTime()) && !Number.isNaN(start.getTime())
      ? Math.max(1, Math.round((end.getTime() - start.getTime()) / 3600000))
      : 1;

  return {
    id: apiLobby.id,
    title:
      apiLobby.lobbyName && apiLobby.lobbyName.trim() !== ""
        ? apiLobby.lobbyName
        : `${apiLobby.ttType} Session`,
    system: apiLobby.ttType,
    sessionNumber: 1,
    date: !Number.isNaN(start.getTime())
      ? start.toLocaleString()
      : apiLobby.startDate,
    dateKey: apiLobby.startDate,
    duration: `${durationHours}h`,
    location: apiLobby.locationName,
    minPlayers: apiLobby.playerMin,
    playerLimit: apiLobby.playerLimit,
    players: users,
    systemColor: systemColorMap[apiLobby.ttType] ?? "#999999",
    dmName: apiLobby.dm,
    description: apiLobby.description ?? "",
    status:
      apiLobby.playerCount >= apiLobby.playerMin ? "confirmed" : "pending",
  };
}

export function mapApiCurrentUserToUser(apiUser: CurrentUserResponse): User {
  return {
    id: apiUser.id,
    name: apiUser.name,
    image:
      apiUser.imageUrl && apiUser.imageUrl.trim() !== ""
        ? apiUser.imageUrl
        : getFallbackProfilePic(apiUser.name),
    rep: apiUser.rep ?? 0,
    role: apiUser.role,
    comments: [],
  };
}

export function mapSearchUserToUser(
  apiUser: SearchUserResponse,
  fallbackId: string
): User {
  return {
    id: apiUser.id || fallbackId,
    name: apiUser.name,
    image:
      apiUser.profileI && apiUser.profileI.trim() !== ""
        ? apiUser.profileI
        : apiUser.profileIUrl && apiUser.profileIUrl.trim() !== ""
        ? apiUser.profileIUrl
        : getFallbackProfilePic(apiUser.name),
    rep: apiUser.rep ?? 0,
    role: apiUser.role,
    comments: [],
  };
}

export function mapProfileCommentsToUserComments(
  comments: ProfileCommentResponse[] | null | undefined
): UserComment[] {
  if (!comments || comments.length === 0) {
    return [];
  }

  return comments.map((comment, index) => ({
    id: `${comment.kommentaloUserId}-${index}`,
    text: comment.kommentSzoveg,
    authorName: comment.name,
  }));
}