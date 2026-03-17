import { apiFetch } from "./api";

export type ApiLocation = {
  id: string;
  locationName: string;
  adress: string;
  description: string;
  image?: string;
};

export async function getLocations() {
  return apiFetch<ApiLocation[]>("/GetLocations");
}