import { useEffect, useMemo, useState } from "react";
import "./App.css";

import LocationCard from "./components/LocationCard";
import LocationDetailsCard from "./components/LocationDetailsCard";
import ReservationDetailsCard from "./components/ReservationDetailsCard";
import MySessionCard from "./components/MySessionCard";
import SessionDetailsCard from "./components/SessionDetailsCard";
import UpcomingSessionCard from "./components/UpcomingSessionCard";
import AuthPage from "./components/AuthPage";
import ProfilePage from "./components/ProfilePage";

import type { Location } from "./types/location";
import type { Session } from "./types/session";
import type { User } from "./types/user";

import questBookLogo from "./assets/branding/QuestBook.png";
import bellIcon from "./assets/icons/bell.png";
import magnifierIcon from "./assets/icons/magnifier.png";
import logoutIcon from "./assets/icons/LogoutIcon.png";

import {
  getCurrentUser,
  login,
  logout,
  register,
  setStoredAccessToken,
} from "./services/authService";
import { getLocations } from "./services/locationService";
import {
  createLobby,
  getAllLobbies,
  getMyLobbies,
  joinLobby,
  leaveLobby,
} from "./services/lobbyService";
import {
  addOrRemoveRep,
  changeUserData,
  deleteComment,
  getUserComments,
  searchUserByName,
  writeComment,
} from "./services/profileService";
import {
  mapApiCurrentUserToUser,
  mapApiLobbyToSession,
  mapApiLocationToLocation,
  mapProfileCommentsToUserComments,
  mapSearchUserToUser,
} from "./services/mappers";

type ActiveTab =
  | "locations"
  | "sessions"
  | "community"
  | "login"
  | "register"
  | "profile";

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
  const [mySessions, setMySessions] = useState<Session[]>([]);
  const [communitySessions, setCommunitySessions] = useState<Session[]>([]);
  const sortedMySessions = useMemo(() => {
    return [...mySessions].sort((a, b) => {
      return new Date(a.dateKey).getTime() - new Date(b.dateKey).getTime();
    });
  }, [mySessions]);

  const sortedCommunitySessions = useMemo(() => {
    return [...communitySessions].sort((a, b) => {
      return new Date(a.dateKey).getTime() - new Date(b.dateKey).getTime();
    });
  }, [communitySessions]);
  const [loggedInUser, setLoggedInUser] = useState<User | null>(null);

  const [selectedProfileUser, setSelectedProfileUser] = useState<User | null>(null);
  const [isProfileEditOpen, setIsProfileEditOpen] = useState(false);
  const [playerSearchTerm, setPlayerSearchTerm] = useState("");
  const [repActions, setRepActions] = useState<Record<string, 1 | -1>>({});

  const [error, setError] = useState("");

  const isAuthScreen = activeTab === "login" || activeTab === "register";
  const currentProfileUser = selectedProfileUser ?? loggedInUser;

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
        const mappedUser = mapApiCurrentUserToUser(apiUser);

        try {
          const apiComments = await getUserComments(mappedUser.id);
          mappedUser.comments = mapProfileCommentsToUserComments(apiComments);
        } catch {
          mappedUser.comments = [];
        }

        setLoggedInUser(mappedUser);
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
          setMySessions(apiLobbies.map(mapApiLobbyToSession));
        }

        if (activeTab === "community") {
          const communityApiLobbies = await getAllLobbies();
          setCommunitySessions(communityApiLobbies.map(mapApiLobbyToSession));
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

  const handleShowProfile = async () => {
    if (!loggedInUser) return;

    try {
      const comments = await getUserComments(loggedInUser.id);

      setSelectedProfileUser({
        ...loggedInUser,
        comments: mapProfileCommentsToUserComments(comments),
      });

      setIsProfileEditOpen(false);
      setActiveTab("profile");
      setError("");
    } catch (error) {
      console.error(error);
      setError("Loading profile failed.");
    }
  };

  const handleAuthSubmit = async (data: {
    mode: "login" | "register";
    username: string;
    email: string;
    password: string;
    passwordAgain?: string;
  }) => {
    try {
      setError("");

      if (data.mode === "register" && data.password !== data.passwordAgain) {
        setError("Passwords do not match.");
        return;
      }

      if (data.mode === "login") {
        await login({
          name: data.username,
          gmail: data.email,
          password: data.password,
        });

        const apiUser = await getCurrentUser();
        const mappedUser = mapApiCurrentUserToUser(apiUser);

        try {
          const apiComments = await getUserComments(mappedUser.id);
          mappedUser.comments = mapProfileCommentsToUserComments(apiComments);
        } catch {
          mappedUser.comments = [];
        }

        setLoggedInUser(mappedUser);
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

        const apiUser = await getCurrentUser();
        const mappedUser = mapApiCurrentUserToUser(apiUser);

        try {
          const apiComments = await getUserComments(mappedUser.id);
          mappedUser.comments = mapProfileCommentsToUserComments(apiComments);
        } catch {
          mappedUser.comments = [];
        }

        setLoggedInUser(mappedUser);
      }

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
    setSelectedProfileUser(null);
    setMySessions([]);
    setCommunitySessions([]);
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
      startDate.setHours(18, 0, 0, 0);

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
      setMySessions(apiLobbies.map(mapApiLobbyToSession));

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

      const refreshedCommunity = await getAllLobbies();
      setCommunitySessions(refreshedCommunity.map(mapApiLobbyToSession));

      if (loggedInUser) {
        const refreshedMine = await getMyLobbies();
        setMySessions(refreshedMine.map(mapApiLobbyToSession));
      }

      setError("");
    } catch (error) {
      console.error(error);
      setError("Joining session failed.");
    }
  };

  const handleLeaveSession = async (sessionId: string) => {
    try {
      await leaveLobby(sessionId);

      const refreshedCommunity = await getAllLobbies();
      setCommunitySessions(refreshedCommunity.map(mapApiLobbyToSession));

      if (loggedInUser) {
        const refreshedMine = await getMyLobbies();
        setMySessions(refreshedMine.map(mapApiLobbyToSession));
      }

      setSelectedSession(null);
      setViewMode("session-list");
      setError("");
    } catch (error) {
      console.error(error);
      setError("Leaving session failed.");
    }
  };

  const handleToggleProfileEdit = () => {
    setIsProfileEditOpen((prev) => !prev);
  };

  const handleBackFromProfile = () => {
    setSelectedProfileUser(null);
    setIsProfileEditOpen(false);
    setActiveTab("locations");
  };

  const handleSearchPlayer = async () => {
    const trimmed = playerSearchTerm.trim();

    if (!trimmed) return;

    try {
      const result = await searchUserByName(trimmed);

      let profileUser = mapSearchUserToUser(
        result,
        `search-${trimmed.toLowerCase()}`
      );

      try {
        if (profileUser.id && !profileUser.id.startsWith("search-")) {
          const comments = await getUserComments(profileUser.id);
          profileUser = {
            ...profileUser,
            comments: mapProfileCommentsToUserComments(comments),
          };
        }
      } catch {
        profileUser.comments = [];
      }

      setSelectedProfileUser(profileUser);
      setIsProfileEditOpen(false);
      setActiveTab("profile");
      setError("");
    } catch (error) {
      console.error(error);
      setError("No player found with that username.");
    }
  };

  const handleRepAction = async (value: 1 | -1) => {
    if (!loggedInUser || !currentProfileUser) return;
    if (loggedInUser.id === currentProfileUser.id) return;
    if (repActions[currentProfileUser.id]) return;

    try {
      if (
        !currentProfileUser.id ||
        currentProfileUser.id.startsWith("search-")
      ) {
        setError("This profile cannot receive rep yet.");
        return;
      }

      await addOrRemoveRep(currentProfileUser.id, value);

      setRepActions((prev) => ({
        ...prev,
        [currentProfileUser.id]: value,
      }));

      setSelectedProfileUser((prev) => {
        if (!prev) return prev;
        return {
          ...prev,
          rep: prev.rep + value,
        };
      });

      setError("");
    } catch (error) {
      console.error(error);
      setError("Rep update failed.");
    }
  };

  const handleDeleteComment = async (commentId: string) => {
    try {
      await deleteComment(commentId);

      if (!selectedProfileUser) return;

      if (!selectedProfileUser.id.startsWith("search-")) {
        const comments = await getUserComments(selectedProfileUser.id);

        const updatedUser = {
          ...selectedProfileUser,
          comments: mapProfileCommentsToUserComments(comments),
        };

        setSelectedProfileUser(updatedUser);

        if (loggedInUser && loggedInUser.id === updatedUser.id) {
          setLoggedInUser(updatedUser);
        }
      }

      setError("");
    } catch (error) {
      console.error(error);
      setError("Deleting comment failed.");
    }
  };

  const handleWriteComment = async (text: string) => {
    if (!selectedProfileUser || !loggedInUser) return;

    try {
      if (selectedProfileUser.id.startsWith("search-")) {
        setError("This profile cannot receive comments yet.");
        return;
      }

      await writeComment({
        fogado: selectedProfileUser.id,
        kommentSzoveg: text,
      });

      const comments = await getUserComments(selectedProfileUser.id);

      setSelectedProfileUser({
        ...selectedProfileUser,
        comments: mapProfileCommentsToUserComments(comments),
      });

      setError("");
    } catch (error) {
      console.error(error);
      setError("Writing comment failed.");
    }
  };

  const handleUpdateProfile = async (data: {
    currentName: string;
    currentPassword: string;
    changeName: string;
    changePassword: string;
    changeProfileI: string;
  }) => {
    if (!loggedInUser) return;

    try {
      const tokenResponse = await changeUserData(data);
      setStoredAccessToken(tokenResponse.accesToken);

      const apiUser = await getCurrentUser();
      const mappedUser = mapApiCurrentUserToUser(apiUser);
      const comments = await getUserComments(mappedUser.id);

      const updatedUser = {
        ...mappedUser,
        comments: mapProfileCommentsToUserComments(comments),
      };

      setLoggedInUser(updatedUser);
      setSelectedProfileUser(updatedUser);
      setIsProfileEditOpen(false);
      setError("");
    } catch (error) {
      console.error(error);
      setError("Profile update failed.");
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

          <div className="navbar-right-section">
            <div className="navbar-search">
              <input
                className="navbar-search-input"
                type="text"
                placeholder="Search player..."
                value={playerSearchTerm}
                onChange={(e) => setPlayerSearchTerm(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    handleSearchPlayer();
                  }
                }}
              />

              <button className="navbar-search-button" onClick={handleSearchPlayer}>
                <img src={magnifierIcon} alt="Search player" />
              </button>
            </div>

            <div className="navbar-icons">
              <img src={bellIcon} alt="Notifications" />

              {loggedInUser && (
                <button className="navbar-icon-button" onClick={handleLogout}>
                  <img src={logoutIcon} alt="Logout" />
                </button>
              )}
            </div>
          </div>
        </header>

        {error && <div className="global-error-banner">{error}</div>}

        <div className="content-layout">
          <main className="main-content">
            <div className="main-overlay">
              {activeTab === "profile" && currentProfileUser && (
                <ProfilePage
                  user={currentProfileUser}
                  viewerId={loggedInUser?.id ?? null}
                  isEditOpen={isProfileEditOpen}
                  hasRepAction={!!repActions[currentProfileUser.id]}
                  onBack={handleBackFromProfile}
                  onToggleEdit={handleToggleProfileEdit}
                  onDeleteComment={handleDeleteComment}
                  onRepAction={handleRepAction}
                  onUpdateProfile={handleUpdateProfile}
                  onWriteComment={handleWriteComment}
                />
              )}

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

              {activeTab === "sessions" && viewMode === "session-list" && (
                <>
                  <h1 className="page-title">My Sessions</h1>

                  <section className="my-sessions-grid">
                    {sortedMySessions.map((session) => (
                      <MySessionCard
                        key={session.id}
                        session={session}
                        onOpen={handleOpenSession}
                        onLeave={handleLeaveSession}
                      />
                    ))}
                  </section>
                </>
              )}

              {activeTab === "community" && viewMode === "session-list" && (
                <>
                  <h1 className="page-title">Community Sessions</h1>

                  <section className="my-sessions-grid">
                    {sortedCommunitySessions.map((session) => (
                      <MySessionCard
                        key={session.id}
                        session={session}
                        onOpen={handleOpenSession}
                        onJoin={handleJoinSession}
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
                    <button
                      className="sidebar-profile-button"
                      onClick={handleShowProfile}
                    >
                      <img
                        src={loggedInUser.image}
                        alt={loggedInUser.name}
                        className="sidebar-user-image"
                      />
                    </button>
                  )}

                  <div className="sidebar-sessions">
                    <h3 className="sidebar-subtitle">Upcoming Reservations</h3>

                    {sortedMySessions.slice(0, 2).map((session) => (
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