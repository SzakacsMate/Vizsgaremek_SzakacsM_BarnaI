import { useState } from "react";
import changeProfileIcon from "../assets/icons/ChangeProfile.png";
import type { User } from "../types/user";

type ProfilePageProps = {
  user: User;
  viewerId: string | null;
  isEditOpen: boolean;
  hasRepAction: boolean;
  onBack: () => void;
  onToggleEdit: () => void;
  onDeleteComment: (commentId: string) => void;
  onRepAction: (value: 1 | -1) => void;
  onUpdateProfile: (data: {
    currentName: string;
    currentPassword: string;
    changeName: string;
    changePassword: string;
    changeProfileI: string;
  }) => void;
  onWriteComment: (text: string) => void;
};

export default function ProfilePage({
  user,
  viewerId,
  isEditOpen,
  hasRepAction,
  onBack,
  onToggleEdit,
  onDeleteComment,
  onRepAction,
  onUpdateProfile,
  onWriteComment,
}: ProfilePageProps) {
  const isOwnProfile = viewerId === user.id;
  const canRep = !!viewerId && !isOwnProfile && !hasRepAction;
  const [newComment, setNewComment] = useState("");

  return (
    <section className="profile-page-card">
      <div className="profile-page-topbar">
        <div className="profile-page-topbar-right">
          {isOwnProfile && (
            <button className="profile-top-action" onClick={onToggleEdit}>
              <img
                src={changeProfileIcon}
                alt="Change profile"
                className="profile-top-action-icon"
              />
              <span>Change Profile</span>
            </button>
          )}

          <button className="details-back-button" onClick={onBack}>
            BACK
          </button>
        </div>
      </div>

      <div className="profile-page-header">
        <div className="profile-page-user">
          <img src={user.image} alt={user.name} className="profile-page-image" />

          <div className="profile-page-user-info">
            <h1 className="profile-page-title">{user.name}</h1>

            <div className="profile-page-rep-row">
              <span className="profile-page-rep-label">REP:</span>
              <span className="profile-page-rep-value">{user.rep}</span>

              {!isOwnProfile && (
                <>
                  <button
                    className="profile-rep-button plus"
                    disabled={!canRep}
                    onClick={() => onRepAction(1)}
                  >
                    +
                  </button>

                  <button
                    className="profile-rep-button minus"
                    disabled={!canRep}
                    onClick={() => onRepAction(-1)}
                  >
                    -
                  </button>
                </>
              )}
            </div>

            {!isOwnProfile && hasRepAction && (
              <p className="profile-rep-note">
                You already used your rep action on this user.
              </p>
            )}
          </div>
        </div>
      </div>

      <div
        className={`profile-layout ${
          isOwnProfile && isEditOpen ? "with-edit" : "single-column"
        }`}
      >
        <div className="profile-comments-card">
          <h2 className="profile-section-title">Received Comments</h2>

          {!isOwnProfile && (
            <div className="profile-comment-create">
              <textarea
                className="profile-comment-textarea"
                placeholder="Write a comment..."
                value={newComment}
                onChange={(e) => setNewComment(e.target.value)}
              />
              <button
                className="profile-save-button"
                onClick={() => {
                  if (!newComment.trim()) return;
                  onWriteComment(newComment.trim());
                  setNewComment("");
                }}
              >
                SEND COMMENT
              </button>
            </div>
          )}

          {user.comments.length === 0 ? (
            <p className="profile-empty-text">No comments yet.</p>
          ) : (
            <div className="profile-comments-list">
              {user.comments.map((comment) => (
                <div key={comment.id} className="profile-comment-item">
                  <p className="profile-comment-author">
                    <strong>{comment.authorName}</strong>
                  </p>
                  <p>{comment.text}</p>

                  {isOwnProfile && (
                    <button
                      className="profile-delete-comment-button"
                      onClick={() => onDeleteComment(comment.id)}
                    >
                      Delete
                    </button>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>

        {isOwnProfile && isEditOpen && (
          <ProfileEditPanel user={user} onUpdateProfile={onUpdateProfile} />
        )}
      </div>
    </section>
  );
}

type ProfileEditPanelProps = {
  user: User;
  onUpdateProfile: (data: {
    currentName: string;
    currentPassword: string;
    changeName: string;
    changePassword: string;
    changeProfileI: string;
  }) => void;
};

function ProfileEditPanel({ user, onUpdateProfile }: ProfileEditPanelProps) {
  const [currentName, setCurrentName] = useState(user.name);
  const [currentPassword, setCurrentPassword] = useState("");
  const [changeName, setChangeName] = useState(user.name);
  const [changePassword, setChangePassword] = useState("");
  const [changeProfileI, setChangeProfileI] = useState(user.image);

  return (
    <div className="profile-edit-card">
      <form
        className="profile-edit-form"
        onSubmit={(event) => {
          event.preventDefault();
          onUpdateProfile({
            currentName,
            currentPassword,
            changeName,
            changePassword,
            changeProfileI,
          });
        }}
      >
        <input
          className="profile-edit-input"
          type="text"
          placeholder="Current name"
          value={currentName}
          onChange={(e) => setCurrentName(e.target.value)}
        />

        <input
          className="profile-edit-input"
          type="password"
          placeholder="Current password"
          value={currentPassword}
          onChange={(e) => setCurrentPassword(e.target.value)}
        />

        <input
          className="profile-edit-input"
          type="text"
          placeholder="New name"
          value={changeName}
          onChange={(e) => setChangeName(e.target.value)}
        />

        <input
          className="profile-edit-input"
          type="text"
          placeholder="New image path / url"
          value={changeProfileI}
          onChange={(e) => setChangeProfileI(e.target.value)}
        />

        <input
          className="profile-edit-input"
          type="password"
          placeholder="New password"
          value={changePassword}
          onChange={(e) => setChangePassword(e.target.value)}
        />

        <button className="profile-save-button" type="submit">
          SAVE CHANGES
        </button>
      </form>
    </div>
  );
}