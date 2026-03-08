import type { Location } from "../types/location";
import type { Session } from "../types/session";
import type { User } from "../types/user";

import tavernImage from "../assets/locations/TAVERNKLUB.jpg";
import jatekcehImage from "../assets/locations/JATEKCEH.jpg";
import demkiImage from "../assets/locations/DEMKI.png";
import profileImage from "../assets/profilepics/ID1.png";

export const locations: Location[] = [
  {
    id: 1,
    name: "TAVERN KLUB",
    address: "Kossuth utca 7, Gambrinus Köz, Debrecen",
    description:
      "A Tavern nem csak egy üzlet – egy hely, amit azoknak hoztunk létre, akik ugyanúgy imádják a kártyajátékokat, társasokat és terepasztalos világokat, mint mi. Egy olyan közösségi tér lesz, ahol lehet játszani, tanulni, versenyezni, ismerkedni, és egyszerűen csak jól érezni magatokat.",
    image: tavernImage,
  },
  {
    id: 2,
    name: "JÁTÉK CÉH",
    address: "Batthyány utca 1, Debrecen",
    description:
      "A Játék Céh egy közösség játékosoknak, akik Magicznek, Yu-Gi-Oh!-znak, Pokémon TCG-nek szerepjátszanak vagy társasjátékoznak.",
    image: jatekcehImage,
  },
  {
    id: 3,
    name: "DEMKI Ifjúsági Ház",
    address: "Simonffy utca 21, Debrecen",
    description:
      "Ez a debreceni központ a fiatalok számára kínál változatos szabadidős programokat és kulturális eseményeket. A Simonffy utcában található épület falai között koncertek, kiállítások és különféle klubok várják a látogatókat.",
    image: demkiImage,
  },
];

export const currentUser: User | null = {
  id: 1,
  name: "Alex",
  image: profileImage,
};

export const upcomingSessions: Session[] = [
  {
    id: 1,
    title: "Curse of Strahd",
    system: "D&D 5e",
    sessionNumber: 7,
    date: "Sat, March 21 - 13:00",
    duration: "4h",
    location: "Tavern - Debrecen",

    playerLimit: 6,
    players: [
      "Boross Bence",
      "Barna István",
      "Szabó Szabolcs",
      "Szász Péter",
      "Vágner Máté",
    ],
    systemColor: "#E53935", //D&D 5e red color
  },
  {
    id: 2,
    title: "Pathfinder: Kingmaker",
    system: "Pathfinder 2e",
    sessionNumber: 3,
    date: "Fri, March 27 - 14:00",
    duration: "5h",
    location: "DEMKI - Debrecen",

    playerLimit: 6,
    players: [
        "Galambos Anna",
        "Buczkó Béla",
        "Géczi Csaba",
        "Nagy Dávid"
    ],
    systemColor: "#F2D600", // Pathfinder 2e yellow color
  },
];