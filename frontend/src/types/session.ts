export type SessionStatus = "confirmed" | "pending";

export type Session = {
  id: string;
  title: string;
  system: string;
  sessionNumber: number;
  date: string;
  dateKey: string;
  duration: string;
  location: string;
  description: string;

  minPlayers: number;
  playerLimit: number;
  players: string[];

  systemColor: string;
  dmName: string;
  status: SessionStatus;
};