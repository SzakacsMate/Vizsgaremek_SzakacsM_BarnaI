import { useEffect, useState } from "react";
import "./App.css";

import LocationCard from "./components/LocationCard";
import LocationDetailsCard from "./components/LocationDetailsCard";
import ReservationDetailsCard from "./components/ReservationDetailsCard";
import MySessionCard from "./components/MySessionCard";
import SessionDetailsCard from "./components/SessionDetailsCard";
import UpcomingSessionCard from "./components/UpcomingSessionCard";
import AuthPage from "./components/AuthPage";

import type { Location } from "./types/location";
import type { Session } from "./types/session";
import type { User } from "./types/user";

import questBookLogo from "./assets/branding/QuestBook.png";
import bellIcon from "./assets/icons/bell.png";
import magnifierIcon from "./assets/icons/magnifier.png";
import logoutIcon from "./assets/icons/LogoutIcon.png";

import { getCurrentUser, login, logout, register } from "./services/authService";
import { getLocations } from "./services/locationService";
import {
  createLobby,
  getAllLobbies,
  getMyLobbies,
  joinLobby,
  leaveLobby,
} from "./services/lobbyService";
import {
  mapApiCurrentUserToUser,
  mapApiLobbyToSession,
  mapApiLocationToLocation,
} from "./services/mappers";

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

function App() {
  const [activeTab, setActiveTab] = useState<ActiveTab>("locations");
  const [viewMode, setViewMode] = useState<ViewMode>("location-list");

  const [selectedLocation, setSelectedLocation] = useState<Location | null>(null);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);

  const [locationsState, setLocationsState] = useState<Location[]>([]);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loggedInUser, setLoggedInUser] = useState<User | null>(null);

  const [error, setError] = useState("");

  const sortedSessions = [...sessions].sort((a, b) => {
    return new Date(a.dateKey).getTime() - new Date(b.dateKey).getTime();
  });

  const isAuthScreen = activeTab === "login" || activeTab === "register";

  useEffect(() => {
    async function loadInitialData() {
      try {
        const apiLocations = await getLocations();
        setLocationsState(apiLocations.map(mapApiLocationToLocation));
      } catch (error) {
        console.error("Locations load error:", error);
      }

      try {
        const apiUser = await getCurrentUser();
        setLoggedInUser(mapApiCurrentUserToUser(apiUser));
      } catch {
        setLoggedInUser(null);
      }
    }

    loadInitialData();
  }, []);

  useEffect(() => {
    async function loadSessions() {
      try {
        if (activeTab === "sessions" && loggedInUser) {
          const apiLobbies = await getMyLobbies();
          setSessions(apiLobbies.map(mapApiLobbyToSession));
        }

        if (activeTab === "community") {
          const apiLobbies = await getAllLobbies();
          setSessions(apiLobbies.map(mapApiLobbyToSession));
        }
      } catch (error) {
        console.error("Sessions load error:", error);
      }
    }

    loadSessions();
  }, [activeTab, loggedInUser]);

  const handleShowLocations = () => {
    setActiveTab("locations");
    setViewMode("location-list");
    setSelectedLocation(null);
    setSelectedSession(null);
    setError("");
  };

  const handleShowSessions = () => {
    if (!loggedInUser) return;
    setActiveTab("sessions");
    setViewMode("session-list");
    setSelectedLocation(null);
    setSelectedSession(null);
    setError("");
  };

  const handleShowCommunity = () => {
    setActiveTab("community");
    setViewMode("session-list");
    setSelectedLocation(null);
    setSelectedSession(null);
    setError("");
  };

  const handleShowLogin = () => {
    setActiveTab("login");
    setError("");
  };

  const handleShowRegister = () => {
    setActiveTab("register");
    setError("");
  };

  const handleAuthSubmit = async (data: {
    mode: "login" | "register";
    username: string;
    email: string;
    password: string;
  }) => {
    try {
      setError("");

      if (data.mode === "login") {
        await login({
          name: data.username,
          gmail: data.email,
          password: data.password,
        });
      } else {
        await register({
          name: data.username,
          gmail: data.email,
          password: data.password,
        });

        await login({
          name: data.username,
          gmail: data.email,
          password: data.password,
        });
      }

      const apiUser = await getCurrentUser();
      setLoggedInUser(mapApiCurrentUserToUser(apiUser));
      setActiveTab("locations");
      setViewMode("location-list");
    } catch (error) {
      console.error(error);
      setError("Login / register failed.");
    }
  };

  const handleLogout = () => {
    logout();
    setLoggedInUser(null);
    setSessions([]);
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

  const handleFinalizeReservation = async (reservationData: {
    location: Location;
    sessionTitle: string;
    gameSystem: string;
    minPlayers: number;
    maxPlayers: number;
    period: number;
    selectedDate: string;
  }) => {
    try {
      const startDate = new Date(reservationData.selectedDate);
      const endDate = new Date(startDate);
      endDate.setHours(endDate.getHours() + reservationData.period);

      await createLobby(reservationData.location.id, {
        lobbyName: reservationData.sessionTitle,
        ttType: reservationData.gameSystem,
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
        playerLimit: reservationData.maxPlayers,
        playerMin: reservationData.minPlayers,
      });

      const apiLobbies = await getMyLobbies();
      setSessions(apiLobbies.map(mapApiLobbyToSession));

      setActiveTab("sessions");
      setViewMode("session-list");
      setSelectedLocation(null);
      setError("");
    } catch (error) {
      console.error(error);
      setError("Creating reservation failed.");
    }
  };

  const handleJoinSession = async (sessionId: string) => {
    try {
      await joinLobby(sessionId);

      const apiLobbies =
        activeTab === "community" ? await getAllLobbies() : await getMyLobbies();

      setSessions(apiLobbies.map(mapApiLobbyToSession));
      setError("");
    } catch (error) {
      console.error(error);
      setError("Joining session failed.");
    }
  };

  const handleLeaveSession = async (sessionId: string) => {
    try {
      await leaveLobby(sessionId);

      const apiLobbies =
        activeTab === "community" ? await getAllLobbies() : await getMyLobbies();

      setSessions(apiLobbies.map(mapApiLobbyToSession));
      setSelectedSession(null);
      setViewMode("session-list");
      setError("");
    } catch (error) {
      console.error(error);
      setError("Leaving session failed.");
    }
  };

  if (isAuthScreen) {
    return (
      <>
        {error && <div className="global-error-banner">{error}</div>}

        <AuthPage
          mode={activeTab === "login" ? "login" : "register"}
          onSwitchMode={(mode) => setActiveTab(mode)}
          onSubmit={handleAuthSubmit}
        />
      </>
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

            <button
              className={`navbar-link-button ${!loggedInUser ? "disabled-link" : ""}`}
              onClick={() => {
                if (loggedInUser) handleShowSessions();
              }}
            >
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

        {error && <div className="global-error-banner">{error}</div>}

        <div className="content-layout">
          <main className="main-content">
            <div className="main-overlay">
              {activeTab === "locations" && viewMode === "location-list" && (
                <>
                  <h1 className="page-title">Locations</h1>

                  <section className="locations-list">
                    {locationsState.map((location) => (
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
                          onJoin={activeTab === "community" ? handleJoinSession : undefined}
                          onLeave={activeTab === "sessions" ? handleLeaveSession : undefined}
                        />
                      ))}
                    </section>
                  </>
                )}

              {viewMode === "session-details" && selectedSession && (
                <SessionDetailsCard
                  session={selectedSession}
                  onBack={handleBackFromSessionDetails}
                  onLeave={handleLeaveSession}
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

                  {loggedInUser.image && (
                    <img
                      src={loggedInUser.image}
                      alt={loggedInUser.name}
                      className="sidebar-user-image"
                    />
                  )}

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