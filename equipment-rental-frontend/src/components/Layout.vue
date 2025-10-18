<template>
  <div class="container-fluid">
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">Equipment Rental</a>
        
        <div class="navbar-nav me-auto">
          <router-link to="/dashboard" class="nav-link">Dashboard</router-link>
          <router-link to="/equipment" class="nav-link">Equipment</router-link>
          <router-link to="/rental" class="nav-link">My Rentals</router-link>
          <router-link v-if="isAdmin" to="/customer" class="nav-link">Customers</router-link>
        </div>
        
        <div class="navbar-nav">
          <span class="navbar-text me-3">
            {{ user?.name }} ({{ user?.role }})
          </span>
          <button @click="logout" class="btn btn-outline-light btn-sm">Logout</button>
        </div>
      </div>
    </nav>
    
    <main class="container-fluid p-4">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const user = computed(() => authStore.user)
const isAdmin = computed(() => user.value?.role === 'Admin')

const logout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
