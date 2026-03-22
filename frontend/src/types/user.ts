export type User = {
  id: string;
  name: string;
  image: string;
  rep: number;
  role?: string;
  comments: UserComment[];
};

export type UserComment = {
  id: string;
  text: string;
  authorName: string;
};
