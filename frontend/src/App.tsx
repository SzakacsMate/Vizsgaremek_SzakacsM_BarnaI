import { useState } from "react";
import "./App.css";
import LocationCard from "./components/LocationCard";
import LocationDetailsCard from "./components/LocationDetailsCard";
import MySessionCard from "./components/MySessionCard";
import ReservationDetailsCard from "./components/ReservationDetailsCard";
import SessionDetailsCard from "./components/SessionDetailsCard";
import UpcomingSessionCard from "./components/UpcomingSessionCard";
import { currentUser, locations, upcomingSessions } from "./data/mockData";
import type { Location } from "./types/location";
import type { Session } from "./types/session";

import questBookLogo from "./assets/branding/QuestBook.png";
import bellIcon from "./assets/icons/bell.png";
import magnifierIcon from "./assets/icons/magnifier.png";

type ActiveTab = "locations" | "sessions";
type ViewMode =
  | "location-list"
  | "location-details"
  | "reservation"
  | "session-list"
  | "session-details";

const systemColorMap: Record<string, string> = {
  "D&D 5e": "#ff1f1f",
  "Pathfinder 2e": "#d6d800",
  "Call of Cthulhu 7e": "#00cc22",
};

function App() {
  const [activeTab, setActiveTab] = useState<ActiveTab>("locations");
  const [viewMode, setViewMode] = useState<ViewMode>("location-list");
  const [selectedLocation, setSelectedLocation] = useState<Location | null>(null);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);
  const [sessions, setSessions] = useState<Session[]>(upcomingSessions);
  const sortedSessions = [...sessions].sort((a, b) => {
  return new Date(a.dateKey).getTime() - new Date(b.dateKey).getTime();
});

  const handleOpenLocation = (location: Location) => {
    setSelectedLocation(location);
    setSelectedSession(null);
    setActiveTab("locations");
    setViewMode("location-details");
  };

  const handleOpenReservation = (location: Location) => {
    setSelectedLocation(location);
    setSelectedSession(null);
    setActiveTab("locations");
    setViewMode("reservation");
  };

  const handleOpenSession = (session: Session) => {
    setSelectedSession(session);
    setSelectedLocation(null);
    setActiveTab("sessions");
    setViewMode("session-details");
  };

  const handleShowLocations = () => {
    setActiveTab("locations");
    setSelectedLocation(null);
    setSelectedSession(null);
    setViewMode("location-list");
  };

  const handleShowSessions = () => {
    setActiveTab("sessions");
    setSelectedLocation(null);
    setSelectedSession(null);
    setViewMode("session-list");
  };

  const handleBackFromLocationDetails = () => {
    setViewMode("location-list");
    setSelectedLocation(null);
  };

  const handleBackFromReservation = () => {
    setViewMode("location-details");
  };

  const handleBackFromSessionDetails = () => {
    setSelectedSession(null);
    setViewMode("session-list");
    setActiveTab("sessions");
  };

  const handleFinalizeReservation = (reservationData: {
    location: Location;
    gameSystem: string;
    maxPlayers: number;
    period: number;
    selectedDate: string;
  }) => {
    const nextSessionNumber =
      sessions.filter((session) => session.title === reservationData.gameSystem).length + 1;

    const newSession: Session = {
      id: Date.now(),
      title: reservationData.gameSystem,
      system: reservationData.gameSystem,
      sessionNumber: nextSessionNumber,
      date: reservationData.selectedDate,
      dateKey: reservationData.selectedDate,
      duration: `${reservationData.period}h`,
      location: reservationData.location.name,
      playerLimit: reservationData.maxPlayers,
      players: [],
      systemColor: systemColorMap[reservationData.gameSystem] ?? "#999999",
      dmName: currentUser?.name ?? "Unknown DM",
      status: "pending",
    };

    setSessions((prev) => [newSession, ...prev]);
    setSelectedSession(null);
    setSelectedLocation(null);
    setActiveTab("sessions");
    setViewMode("session-list");
  };

  return (
    <div className="page">
      <div className="app-shell">
        <header className="navbar">
          <div className="navbar-logo-section">
            <img
              src={questBookLogo}
              alt="QuestBook"
              className="navbar-logo"
            />
          </div>

          <div className="navbar-divider" />

          <nav className="navbar-links">
            <button
              className="navbar-link-button"
              onClick={handleShowLocations}
            >
              Locations
            </button>

            <button
              className="navbar-link-button"
              onClick={handleShowSessions}
            >
              My Sessions
            </button>

            <button className="navbar-link-button">Community</button>
          </nav>

          <div className="navbar-divider" />

          <div className="navbar-icons">
            <img src={bellIcon} alt="Notifications" />
            <img src={magnifierIcon} alt="Search" />
          </div>
        </header>

        <div className="content-layout">
          <main className="main-content">
            <div className="main-overlay">
              {activeTab === "locations" && viewMode === "location-list" && (
                <>
                  <h1 className="page-title">Locations</h1>

                  <section className="locations-list">
                    {locations.map((location) => (
                      <LocationCard
                        key={location.id}
                        location={location}
                        onOpen={handleOpenLocation}
                        onReserve={handleOpenReservation}
                      />
                    ))}
                  </section>
                </>
              )}

              {activeTab === "locations" &&
                viewMode === "location-details" &&
                selectedLocation !== null && (
                  <LocationDetailsCard
                    location={selectedLocation}
                    onBack={handleBackFromLocationDetails}
                    onReserve={handleOpenReservation}
                  />
                )}

              {activeTab === "locations" &&
                viewMode === "reservation" &&
                selectedLocation !== null && (
                  <ReservationDetailsCard
                    location={selectedLocation}
                    onBack={handleBackFromReservation}
                    onFinalize={handleFinalizeReservation}
                  />
                )}

              {activeTab === "sessions" && viewMode === "session-list" && (
                <>
                  <h1 className="page-title">My Sessions</h1>

                  <section className="my-sessions-grid">
                    {sortedSessions.map((session) => (
                      <MySessionCard
                        key={session.id}
                        session={session}
                        onOpen={handleOpenSession}
                      />
                    ))}
                  </section>
                </>
              )}

              {activeTab === "sessions" &&
                viewMode === "session-details" &&
                selectedSession !== null && (
                  <SessionDetailsCard
                    session={selectedSession}
                    onBack={handleBackFromSessionDetails}
                  />
                )}
            </div>
          </main>

          <aside className="sidebar">
            <div className="sidebar-overlay">
              {currentUser ? (
                <>
                  <h2 className="sidebar-title">
                    Welcome back, {currentUser.name}!
                  </h2>

                  <img
                    src={currentUser.image}
                    alt={currentUser.name}
                    className="sidebar-user-image"
                  />
                </>
              ) : (
                <h2 className="sidebar-title">
                  You are currently not logged in.
                </h2>
              )}

              <div className="sidebar-sessions">
                <h3 className="sidebar-subtitle">Upcoming Reservations</h3>

                {sortedSessions.slice(0, 2).map((session) => (
                  <UpcomingSessionCard key={session.id} session={session} />
                ))}
              </div>
            </div>
          </aside>
        </div>
      </div>
    </div>
  );
}

export default App;