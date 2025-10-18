<template>
  <div class="container-fluid vh-100 d-flex align-items-center justify-content-center bg-light">
    <div class="card shadow" style="width: 400px;">
      <div class="card-body p-5">
        <h2 class="text-center mb-4">Equipment Rental</h2>
        <h4 class="text-center mb-4">Login</h4>
        
        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label for="username" class="form-label">Username</label>
            <input
              type="text"
              class="form-control"
              id="username"
              v-model="credentials.username"
              required
            />
          </div>
          
          <div class="mb-3">
            <label for="password" class="form-label">Password</label>
            <input
              type="password"
              class="form-control"
              id="password"
              v-model="credentials.password"
              required
            />
          </div>
          
          <div v-if="error" class="alert alert-danger" role="alert">
            {{ error }}
          </div>
          
          <button 
            type="submit" 
            class="btn btn-primary w-100"
            :disabled="loading"
          >
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            {{ loading ? 'Logging in...' : 'Login' }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const credentials = ref({
  username: '',
  password: ''
})

const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  const result = await authStore.login(credentials.value)
  
  if (result.success) {
    router.push('/dashboard')
  } else {
    error.value = result.error
  }
  
  loading.value = false
}
</script>
