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
    bookedDates: ["2026-12-12", "2026-12-18", "2026-12-25"],
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
    bookedDates: ["2026-12-10", "2026-12-14", "2026-12-22"],
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
    bookedDates: ["2026-12-10", "2026-12-14", "2026-12-22"],
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
    dateKey: "2026-03-21",
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
    dmName: "Varjú Bálint",
    status: "confirmed",
    systemColor: "#ff1f1f",
  },
  {
    id: 2,
    title: "Pathfinder: Kingmaker",
    system: "Pathfinder 2e",
    sessionNumber: 3,
    date: "Fri, March 27 - 14:00",
    dateKey: "2026-03-27",
    duration: "5h",
    location: "Demki - Debrecen",
    playerLimit: 6,
    players: ["Anna", "Béla", "Csaba", "Dávid"],
    dmName: "Sándor Péter",
    status: "confirmed",
    systemColor: "#d6d800",
  },
  {
    id: 3,
    title: "Masks of Nyarlathotep",
    system: "Call of Cthulhu 7e",
    sessionNumber: 23,
    date: "Sat, March 28 - 12:30",
    dateKey: "2026-03-28",
    duration: "5h",
    location: "Tavern - Debrecen",
    playerLimit: 6,
    players: ["Bence", "Peti", "Máté"],
    dmName: "Boross Bence",
    status: "pending",
    systemColor: "#00cc22",
  },
  {
    id: 4,
    title: "Curse of Strahd",
    system: "D&D 5e",
    sessionNumber: 8,
    date: "Sat, April 4 - 12:30",
    dateKey: "2026-04-04",
    duration: "6h",
    location: "Játé Céh - Debrecen",
    playerLimit: 6,
    players: ["Ádám", "Levi", "Noel", "Márk"],
    dmName: "Varjú Bálint",
    status: "confirmed",
    systemColor: "#ff1f1f",
  },
];