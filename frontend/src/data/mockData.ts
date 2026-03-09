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
      "A Tavern nem csak egy üzlet – egy hely, amit azoknak hoztunk létre, akik ugyanúgy imádják a kártyajátékokat, társasokat és terepasztalos világokat, mint mi.",
    longDescription:
      "A Tavern nem csak egy üzlet – egy hely, amit azoknak hoztunk létre, akik ugyanúgy imádják a kártyajátékokat, társasokat és terepasztalos világokat, mint mi. Egy olyan közösségi tér lesz, ahol lehet játszani, tanulni, versenyezni, ismerkedni, és egyszerűen csak jól érezni magatokat. Otthont adunk játékos közösségeknek, játéknapoknak, versenyeknek és sok-sok jövőbeli eseménynek.\n\nA boltban a játékok mellett találtok kártyás kiegészítőket, makettezéshez szükséges eszközöket, könyveket és minden olyan apróságot, amire egy igazi játékosnak szüksége lehet – legyen szó kezdőről vagy veteránról. Ha kérdésetek van, tanácsra van szükségetek, vagy csak beszélgetnétek egy jót egy új pakliról, figuráról vagy stratégiáról, mi itt leszünk.",
    image: tavernImage,
  },
  {
    id: 2,
    name: "JÁTÉK CÉH",
    address: "Batthyány utca 1, Debrecen",
    description:
      "A Játék Céh egy közösség játékosoknak, akik Magicznek, Yu-Gi-Oh!-znak, Pokémon TCG-nek szerepjátszanak vagy társasjátékoznak.",
    longDescription:
      "A Játék Céh egy közösség játékosoknak, akik Magicznek, Yu-Gi-Oh!-znak, Pokémon TCG-nek szerepjátszanak vagy társasjátékoznak. Barátságos közeg, kisebb csapatoknak és rendszeres összejövetelekhez nagyon jó választás.\n\nA hely hangulata közvetlen, könnyű beülni egy kampányra, one-shotra vagy akár csak egy laza társasos estére. Ha egy könnyen megközelíthető, játékosbarát helyszínt kerestek, ez jó opció.",
    image: jatekcehImage,
  },
  {
    id: 3,
    name: "DEMKI Ifjúsági Ház",
    address: "Simonffy utca 21, Debrecen",
    description:
      "Ez a debreceni központ a fiatalok számára kínál változatos szabadidős programokat és kulturális eseményeket.",
    longDescription:
      "Ez a debreceni központ a fiatalok számára kínál változatos szabadidős programokat és kulturális eseményeket. A Simonffy utcában található épület falai között koncertek, kiállítások és különféle klubok várják a látogatókat.\n\nNagyobb helyszín, ezért több asztalos eseményekhez, közösségi programokhoz és nyílt játékalkalmakhoz is jól használható. Ha tágasabb teret kerestek, ez lesz a legjobb opció.",
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
    systemColor: "#ff1f1f",
  },
  {
    id: 2,
    title: "Pathfinder: Kingmaker",
    system: "Pathfinder 2e",
    sessionNumber: 3,
    date: "Fri, March 27 - 14:00",
    duration: "5h",
    location: "Demki - Debrecen",
    playerLimit: 6,
    players: ["Anna", "Béla", "Csaba", "Dávid"],
    systemColor: "#d6d800",
  },
  {
    id: 3,
    title: "Masks of Nyarlathotep",
    system: "Call of Cthulhu 7e",
    sessionNumber: 23,
    date: "Sat, March 28 - 12:30",
    duration: "5h",
    location: "Tavern - Debrecen",
    playerLimit: 6,
    players: ["Bence", "Peti", "Máté"],
    systemColor: "#00cc22",
  },
  {
    id: 4,
    title: "Curse of Strahd",
    system: "D&D 5e",
    sessionNumber: 8,
    date: "Sat, April 4 - 12:30",
    duration: "6h",
    location: "Játé Céh - Debrecen",
    playerLimit: 6,
    players: ["Ádám", "Levi", "Noel", "Márk"],
    systemColor: "#ff1f1f",
  },
];