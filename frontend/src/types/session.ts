export type SessionStatus = "confirmed" | "pending";

export type Session = {
  id: number;
  title: string;
  system: string;
  sessionNumber: number;
  date: string;
  dateKey: string;
  duration: string;
  location: string;
  playerLimit: number;
  players: string[];
  systemColor: string;
  dmName: string;
  status: SessionStatus;
};