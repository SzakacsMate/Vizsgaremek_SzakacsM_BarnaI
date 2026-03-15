import { useState } from "react";
import "./App.css";

import LocationCard from "./components/LocationCard";
import LocationDetailsCard from "./components/LocationDetailsCard";
import ReservationDetailsCard from "./components/ReservationDetailsCard";
import MySessionCard from "./components/MySessionCard";
import SessionDetailsCard from "./components/SessionDetailsCard";
import UpcomingSessionCard from "./components/UpcomingSessionCard";
import AuthPage from "./components/AuthPage";

import { currentUser, locations, upcomingSessions } from "./data/mockData";

import type { Location } from "./types/location";
import type { Session } from "./types/session";

import questBookLogo from "./assets/branding/QuestBook.png";
import bellIcon from "./assets/icons/bell.png";
import magnifierIcon from "./assets/icons/magnifier.png";
import logoutIcon from "./assets/icons/LogoutIcon.png";

type ActiveTab =
  | "locations"
  | "sessions"
  | "community"
  | "login"
  | "register";

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

  const [sessions, setSessions] = useState<Session[]>(
    upcomingSessions.map((session) => ({
      ...session,
      status:
        session.players.length >= session.minPlayers ? "confirmed" : "pending",
    }))
  );
  const [loggedInUser, setLoggedInUser] = useState(currentUser);

  const sortedSessions = [...sessions].sort((a, b) => {
    return new Date(a.dateKey).getTime() - new Date(b.dateKey).getTime();
  });

  const isAuthScreen = activeTab === "login" || activeTab === "register";

  const handleShowLocations = () => {
    setActiveTab("locations");
    setViewMode("location-list");
    setSelectedLocation(null);
    setSelectedSession(null);
  };

  const handleShowSessions = () => {
    setActiveTab("sessions");
    setViewMode("session-list");
    setSelectedLocation(null);
    setSelectedSession(null);
  };

  const handleShowCommunity = () => {
    setActiveTab("community");
    setViewMode("session-list");
    setSelectedLocation(null);
    setSelectedSession(null);
  };

  const handleShowLogin = () => {
    setActiveTab("login");
  };

  const handleShowRegister = () => {
    setActiveTab("register");
  };

  const handleLoginSuccess = (username: string) => {
    setLoggedInUser({
      id: Date.now(),
      name: username,
      image: currentUser?.image ?? "",
    });

    setActiveTab("locations");
    setViewMode("location-list");
  };

  const handleLogout = () => {
    setLoggedInUser(null);
    setActiveTab("locations");
    setViewMode("location-list");
    setSelectedLocation(null);
    setSelectedSession(null);
  };

  const handleOpenLocation = (location: Location) => {
    setSelectedLocation(location);
    setViewMode("location-details");
  };

  const handleOpenReservation = (location: Location) => {
    setSelectedLocation(location);
    setViewMode("reservation");
  };

  const handleBackFromLocationDetails = () => {
    setSelectedLocation(null);
    setViewMode("location-list");
  };

  const handleBackFromReservation = () => {
    setViewMode("location-details");
  };

  const handleOpenSession = (session: Session) => {
    setSelectedSession(session);
    setViewMode("session-details");
  };

  const handleBackFromSessionDetails = () => {
    setSelectedSession(null);
    setViewMode("session-list");
  };

  const handleFinalizeReservation = (reservationData: {
    location: Location;
    sessionTitle: string;
    gameSystem: string;
    minPlayers: number;
    maxPlayers: number;
    period: number;
    selectedDate: string;
  }) => {
    const newSession: Session = {
      id: Date.now(),
      title: reservationData.sessionTitle,
      system: reservationData.gameSystem,
      sessionNumber: 1,
      date: reservationData.selectedDate,
      dateKey: reservationData.selectedDate,
      duration: `${reservationData.period}h`,
      location: reservationData.location.name,
      minPlayers: reservationData.minPlayers,
      playerLimit: reservationData.maxPlayers,
      players: [],
      systemColor: systemColorMap[reservationData.gameSystem] ?? "#999",
      dmName: loggedInUser?.name ?? "Unknown",
      status: "pending",
    };

    setSessions((prev) => [newSession, ...prev]);
    setActiveTab("sessions");
    setViewMode("session-list");
    setSelectedLocation(null);
  };

  if (isAuthScreen) {
    return (
      <AuthPage
        mode={activeTab === "login" ? "login" : "register"}
        onSwitchMode={(mode) => setActiveTab(mode)}
        onLoginSuccess={handleLoginSuccess}
      />
    );
  }

  return (
    <div className="page">
      <div className="app-shell">
        <header className="navbar">
          <div className="navbar-logo-section">
            <img src={questBookLogo} alt="QuestBook" className="navbar-logo" />
          </div>

          <div className="navbar-divider" />

          <nav className="navbar-links">
            <button className="navbar-link-button" onClick={handleShowLocations}>
              Locations
            </button>

            <button className={`navbar-link-button ${!loggedInUser ? "disabled-link" : ""}`} onClick={() => {
              if (loggedInUser) handleShowSessions();
            }}>
              My Sessions
            </button>

            <button className="navbar-link-button" onClick={handleShowCommunity}>
              Community
            </button>
          </nav>

          <div className="navbar-divider" />

          <div className="navbar-icons">
            <img src={bellIcon} alt="Notifications" />
            <img src={magnifierIcon} alt="Search" />

            {loggedInUser && (
              <button className="navbar-icon-button" onClick={handleLogout}>
                <img src={logoutIcon} alt="Logout" />
              </button>
            )}
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
                        canReserve={!!loggedInUser}
                      />
                    ))}
                  </section>
                </>
              )}

              {viewMode === "location-details" && selectedLocation && (
                <LocationDetailsCard
                  location={selectedLocation}
                  onBack={handleBackFromLocationDetails}
                  onReserve={handleOpenReservation}
                  canReserve={!!loggedInUser}
                />
              )}

              {viewMode === "reservation" && selectedLocation && (
                <ReservationDetailsCard
                  location={selectedLocation}
                  onBack={handleBackFromReservation}
                  onFinalize={handleFinalizeReservation}
                />
              )}

              {(activeTab === "sessions" || activeTab === "community") &&
                viewMode === "session-list" && (
                  <>
                    <h1 className="page-title">
                      {activeTab === "sessions"
                        ? "My Sessions"
                        : "Community Sessions"}
                    </h1>

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

              {viewMode === "session-details" && selectedSession && (
                <SessionDetailsCard
                  session={selectedSession}
                  onBack={handleBackFromSessionDetails}
                />
              )}
            </div>
          </main>

          <aside className="sidebar">
            <div className="sidebar-overlay">
              {loggedInUser ? (
                <>
                  <h2 className="sidebar-title">
                    Welcome back, {loggedInUser.name}!
                  </h2>

                  <img
                    src={loggedInUser.image}
                    alt={loggedInUser.name}
                    className="sidebar-user-image"
                  />

                  <div className="sidebar-sessions">
                    <h3 className="sidebar-subtitle">Upcoming Reservations</h3>

                    {sortedSessions.slice(0, 2).map((session) => (
                      <UpcomingSessionCard key={session.id} session={session} />
                    ))}
                  </div>
                </>
              ) : (
                <div className="sidebar-auth-card">
                  <h2 className="sidebar-auth-title">
                    You are currently not logged in.
                  </h2>

                  <p className="sidebar-auth-text">
                    Login or create an account to reserve places and manage your sessions.
                  </p>

                  <div className="sidebar-auth-actions">
                    <button
                      className="sidebar-auth-button"
                      onClick={handleShowLogin}
                    >
                      LOGIN
                    </button>

                    <button
                      className="sidebar-auth-button secondary"
                      onClick={handleShowRegister}
                    >
                      REGISTER
                    </button>
                  </div>
                </div>
              )}
            </div>
          </aside>
        </div>
      </div>
    </div>
  );
}

export default App;