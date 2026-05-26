import LoginForm from '../../components/auth/LoginForm';

export default function LoginPage() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <h1 className="text-2xl font-bold mb-6">Log in</h1>
      <LoginForm />
    </div>
  );
}
