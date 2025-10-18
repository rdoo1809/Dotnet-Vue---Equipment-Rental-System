# Vue Frontend Implementation Guide
## Equipment Rental Management System

This guide provides step-by-step instructions for implementing a Vue 3 frontend for the existing .NET 9.0 Web API backend.

## Prerequisites
- Node.js (v18 or higher)
- npm or yarn package manager
- Backend API running on `http://localhost:5129`
- Basic knowledge of Vue 3, JavaScript, and HTTP APIs

---

## Phase 1: Project Setup & Configuration

### Step 1.1: Initialize Vue Project
```bash
# Create new Vue 3 project with Vite
npm create vue@latest equipment-rental-frontend
cd equipment-rental-frontend

# Select options:
# - TypeScript: No
# - JSX: No
# - Vue Router: Yes
# - Pinia: Yes
# - Vitest: No
# - E2E Testing: No
# - ESLint: Yes
# - Prettier: Yes
```

### Step 1.2: Install Additional Dependencies
```bash
npm install axios
npm install @vueuse/core
npm install bootstrap
npm install bootstrap-icons
```

### Step 1.3: Configure Bootstrap
```javascript
// main.js
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.mount('#app')
```

### Step 1.4: Create API Service
```javascript
// src/services/api.js
import axios from 'axios'

const API_BASE = 'http://localhost:5129/api'

const api = axios.create({
  baseURL: API_BASE,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api
```

### Step 1.5: Create Auth Store
```javascript
// src/stores/auth.js
import { defineStore } from 'pinia'
import api from '@/services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token'),
    isAuthenticated: !!localStorage.getItem('token')
  }),

  actions: {
    async login(credentials) {
      try {
        const response = await api.post('/auth/login', credentials)
        const { token, user } = response.data
        
        this.token = token
        this.user = user
        this.isAuthenticated = true
        
        localStorage.setItem('token', token)
        return { success: true }
      } catch (error) {
        return { 
          success: false, 
          error: error.response?.data?.message || 'Login failed' 
        }
      }
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      localStorage.removeItem('token')
    },

    async getUserInfo() {
      try {
        const response = await api.get('/customers/me')
        this.user = response.data
        return response.data
      } catch (error) {
        console.error('Failed to get user info:', error)
        return null
      }
    }
  }
})
```

### Step 1.6: Configure Router with Guards
```javascript
// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { requiresGuest: true }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/Dashboard.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/equipment',
    name: 'Equipment',
    component: () => import('@/views/Equipment.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/equipment/:id',
    name: 'EquipmentDetails',
    component: () => import('@/views/EquipmentDetails.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/customers',
    name: 'Customers',
    component: () => import('@/views/Customers.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/customers/:id',
    name: 'CustomerDetails',
    component: () => import('@/views/CustomerDetails.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/rentals',
    name: 'Rentals',
    component: () => import('@/views/Rentals.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/rentals/:id',
    name: 'RentalDetails',
    component: () => import('@/views/RentalDetails.vue'),
    meta: { requiresAuth: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next('/dashboard')
    return
  }
  
  if (to.meta.requiresAdmin && authStore.user?.role !== 'Admin') {
    next('/dashboard')
    return
  }
  
  next()
})

export default router
```

---

## Phase 2: Authentication Implementation

### Step 2.1: Create Login Component
```vue
<!-- src/views/Login.vue -->
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
    await authStore.getUserInfo()
    router.push('/dashboard')
  } else {
    error.value = result.error
  }
  
  loading.value = false
}
</script>
```

### Step 2.2: Create Layout Component
```vue
<!-- src/components/Layout.vue -->
<template>
  <div class="container-fluid">
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">Equipment Rental</a>
        
        <div class="navbar-nav me-auto">
          <router-link to="/dashboard" class="nav-link">Dashboard</router-link>
          <router-link to="/equipment" class="nav-link">Equipment</router-link>
          <router-link to="/rentals" class="nav-link">My Rentals</router-link>
          <router-link v-if="isAdmin" to="/customers" class="nav-link">Customers</router-link>
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
```

### Step 2.3: Update App.vue
```vue
<!-- src/App.vue -->
<template>
  <div id="app">
    <Layout v-if="isAuthenticated" />
    <router-view v-else />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Layout from '@/components/Layout.vue'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
</script>
```

---

## Phase 3: Dashboard Implementation

### Step 3.1: Create Dashboard Service
```javascript
// src/services/dashboard.js
import api from './api'

export const dashboardService = {
  async getEquipmentStats() {
    const [total, available, rented, overdue] = await Promise.all([
      api.get('/equipment').then(res => res.data.length),
      api.get('/equipment/available').then(res => res.data.length),
      api.get('/equipment/rented').then(res => res.data.length),
      api.get('/rentals/overdue').then(res => res.data.length)
    ])
    
    return { total, available, rented, overdue }
  },

  async getActiveRentals() {
    const response = await api.get('/rentals/active')
    return response.data
  },

  async getUserActiveRental() {
    const response = await api.get('/customers/me/active-rental')
    return response.data
  }
}
```

### Step 3.2: Create Dashboard Component
```vue
<!-- src/views/Dashboard.vue -->
<template>
  <div>
    <h1 class="mb-4">Equipment Rental Dashboard</h1>
    <p class="text-muted mb-4">Welcome to the Equipment Rental Management System.</p>
    
    <!-- Stats Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card text-white bg-primary">
          <div class="card-body">
            <h2 class="card-title">{{ stats.total }}</h2>
            <p class="card-text">Total Equipment</p>
            <router-link to="/equipment" class="btn btn-light">View All</router-link>
          </div>
        </div>
      </div>
      
      <div class="col-md-3">
        <div class="card text-white bg-success">
          <div class="card-body">
            <h2 class="card-title">{{ stats.available }}</h2>
            <p class="card-text">Available</p>
            <router-link to="/equipment" class="btn btn-light">View Available</router-link>
          </div>
        </div>
      </div>
      
      <div class="col-md-3">
        <div class="card text-white bg-warning">
          <div class="card-body">
            <h2 class="card-title">{{ stats.rented }}</h2>
            <p class="card-text">Currently Rented</p>
            <router-link to="/rentals" class="btn btn-light">View Rented</router-link>
          </div>
        </div>
      </div>
      
      <div class="col-md-3">
        <div class="card text-white bg-danger">
          <div class="card-body">
            <h2 class="card-title">{{ stats.overdue }}</h2>
            <p class="card-text">Overdue Rentals</p>
            <small v-if="!isAdmin">Admin Access Required</small>
            <router-link v-else to="/rentals?filter=overdue" class="btn btn-light">View Overdue</router-link>
          </div>
        </div>
      </div>
    </div>
    
    <div class="row">
      <!-- Quick Actions -->
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5>Quick Actions</h5>
          </div>
          <div class="card-body">
            <div class="d-grid gap-2">
              <button @click="showIssueModal = true" class="btn btn-primary">Issue Equipment</button>
              <button @click="showReturnModal = true" class="btn btn-success">Return Equipment</button>
              <router-link to="/rentals" class="btn btn-info">View My Rentals</router-link>
            </div>
          </div>
        </div>
      </div>
      
      <!-- System Status -->
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5>System Status</h5>
          </div>
          <div class="card-body">
            <p><strong>Active Rentals:</strong> {{ activeRentals.length }}</p>
            <p><strong>Overdue Rentals:</strong> {{ stats.overdue }}</p>
            <p><strong>Logged in as:</strong> {{ user?.name }} ({{ user?.role }})</p>
            <p><strong>System Status:</strong> <span class="text-success">Online</span></p>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Issue Equipment Modal -->
    <IssueEquipmentModal 
      v-if="showIssueModal" 
      @close="showIssueModal = false"
      @success="handleIssueSuccess"
    />
    
    <!-- Return Equipment Modal -->
    <ReturnEquipmentModal 
      v-if="showReturnModal" 
      @close="showReturnModal = false"
      @success="handleReturnSuccess"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { dashboardService } from '@/services/dashboard'
import IssueEquipmentModal from '@/components/IssueEquipmentModal.vue'
import ReturnEquipmentModal from '@/components/ReturnEquipmentModal.vue'

const authStore = useAuthStore()
const user = computed(() => authStore.user)
const isAdmin = computed(() => user.value?.role === 'Admin')

const stats = ref({
  total: 0,
  available: 0,
  rented: 0,
  overdue: 0
})

const activeRentals = ref([])
const showIssueModal = ref(false)
const showReturnModal = ref(false)

onMounted(async () => {
  await loadDashboardData()
})

const loadDashboardData = async () => {
  try {
    const [statsData, activeRentalsData] = await Promise.all([
      dashboardService.getEquipmentStats(),
      dashboardService.getActiveRentals()
    ])
    
    stats.value = statsData
    activeRentals.value = activeRentalsData
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  }
}

const handleIssueSuccess = () => {
  showIssueModal.value = false
  loadDashboardData()
}

const handleReturnSuccess = () => {
  showReturnModal.value = false
  loadDashboardData()
}
</script>
```

---

## Phase 4: Equipment Management

### Step 4.1: Create Equipment Service
```javascript
// src/services/equipment.js
import api from './api'

export const equipmentService = {
  async getAll() {
    const response = await api.get('/equipment')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/equipment/${id}`)
    return response.data
  },

  async getAvailable() {
    const response = await api.get('/equipment/available')
    return response.data
  },

  async getRented() {
    const response = await api.get('/equipment/rented')
    return response.data
  },

  async create(equipment) {
    const response = await api.post('/equipment', equipment)
    return response.data
  },

  async update(id, equipment) {
    const response = await api.put(`/equipment/${id}`, equipment)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/equipment/${id}`)
    return response.data
  }
}
```

### Step 4.2: Create Equipment List Component
```vue
<!-- src/views/Equipment.vue -->
<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Equipment</h1>
      <button 
        v-if="isAdmin" 
        @click="showCreateModal = true" 
        class="btn btn-primary"
      >
        Add Equipment
      </button>
    </div>
    
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Name</th>
                <th>Category</th>
                <th>Condition</th>
                <th>Status</th>
                <th v-if="isAdmin">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="equipment in equipmentList" :key="equipment.id">
                <td>{{ equipment.name }}</td>
                <td>{{ equipment.category }}</td>
                <td>{{ equipment.condition }}</td>
                <td>
                  <span :class="getStatusClass(equipment.status)">
                    {{ equipment.status }}
                  </span>
                </td>
                <td v-if="isAdmin">
                  <div class="btn-group" role="group">
                    <router-link 
                      :to="`/equipment/${equipment.id}`" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </router-link>
                    <button 
                      @click="editEquipment(equipment)" 
                      class="btn btn-sm btn-outline-secondary"
                    >
                      Edit
                    </button>
                    <button 
                      @click="deleteEquipment(equipment)" 
                      class="btn btn-sm btn-outline-danger"
                    >
                      Delete
                    </button>
                  </div>
                </td>
                <td v-else>
                  <router-link 
                    :to="`/equipment/${equipment.id}`" 
                    class="btn btn-sm btn-primary"
                  >
                    Details
                  </router-link>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!-- Create/Edit Modal -->
    <EquipmentModal 
      v-if="showCreateModal || showEditModal"
      :equipment="editingEquipment"
      @close="closeModal"
      @save="handleSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Equipment"
      message="Are you sure you want to delete this equipment?"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { equipmentService } from '@/services/equipment'
import EquipmentModal from '@/components/EquipmentModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const equipmentList = ref([])
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const editingEquipment = ref(null)
const deletingEquipment = ref(null)

onMounted(() => {
  loadEquipment()
})

const loadEquipment = async () => {
  try {
    equipmentList.value = await equipmentService.getAll()
  } catch (error) {
    console.error('Failed to load equipment:', error)
  }
}

const getStatusClass = (status) => {
  const classes = {
    'Available': 'badge bg-success',
    'Rented': 'badge bg-warning',
    'Maintenance': 'badge bg-danger'
  }
  return classes[status] || 'badge bg-secondary'
}

const editEquipment = (equipment) => {
  editingEquipment.value = equipment
  showEditModal.value = true
}

const deleteEquipment = (equipment) => {
  deletingEquipment.value = equipment
  showDeleteModal.value = true
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingEquipment.value = null
}

const handleSave = async (equipmentData) => {
  try {
    if (editingEquipment.value) {
      await equipmentService.update(editingEquipment.value.id, equipmentData)
    } else {
      await equipmentService.create(equipmentData)
    }
    closeModal()
    loadEquipment()
  } catch (error) {
    console.error('Failed to save equipment:', error)
  }
}

const confirmDelete = async () => {
  try {
    await equipmentService.delete(deletingEquipment.value.id)
    showDeleteModal.value = false
    deletingEquipment.value = null
    loadEquipment()
  } catch (error) {
    console.error('Failed to delete equipment:', error)
  }
}
</script>
```

### Step 4.3: Create Equipment Details Component
```vue
<!-- src/views/EquipmentDetails.vue -->
<template>
  <div v-if="equipment">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>{{ equipment.name }}</h1>
      <div>
        <button 
          v-if="isAdmin" 
          @click="editEquipment" 
          class="btn btn-secondary me-2"
        >
          Edit
        </button>
        <button 
          v-if="isAdmin" 
          @click="deleteEquipment" 
          class="btn btn-danger me-2"
        >
          Delete
        </button>
        <button 
          v-if="canIssue" 
          @click="issueEquipment" 
          class="btn btn-primary"
        >
          Issue Equipment
        </button>
      </div>
    </div>
    
    <div class="row">
      <div class="col-md-8">
        <div class="card">
          <div class="card-header">
            <h5>Equipment Details</h5>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-6">
                <p><strong>Name:</strong> {{ equipment.name }}</p>
                <p><strong>Category:</strong> {{ equipment.category }}</p>
                <p><strong>Condition:</strong> {{ equipment.condition }}</p>
              </div>
              <div class="col-md-6">
                <p><strong>Status:</strong> 
                  <span :class="getStatusClass(equipment.status)">
                    {{ equipment.status }}
                  </span>
                </p>
                <p><strong>Description:</strong> {{ equipment.description }}</p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Rental History (Admin only) -->
        <div v-if="isAdmin" class="card mt-4">
          <div class="card-header">
            <h5>Rental History</h5>
          </div>
          <div class="card-body">
            <div v-if="rentalHistory.length === 0" class="text-muted">
              No rental history available
            </div>
            <div v-else>
              <div v-for="rental in rentalHistory" :key="rental.id" class="border-bottom py-2">
                <div class="d-flex justify-content-between">
                  <span>{{ rental.customerName }}</span>
                  <span class="text-muted">{{ formatDate(rental.issuedAt) }} - {{ formatDate(rental.returnedAt) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Issue Equipment Modal -->
    <IssueEquipmentModal 
      v-if="showIssueModal"
      :equipment="equipment"
      @close="showIssueModal = false"
      @success="handleIssueSuccess"
    />
    
    <!-- Edit Modal -->
    <EquipmentModal 
      v-if="showEditModal"
      :equipment="equipment"
      @close="showEditModal = false"
      @save="handleEditSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Equipment"
      message="Are you sure you want to delete this equipment?"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
  
  <div v-else class="text-center">
    <div class="spinner-border" role="status">
      <span class="visually-hidden">Loading...</span>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { equipmentService } from '@/services/equipment'
import { rentalService } from '@/services/rental'
import IssueEquipmentModal from '@/components/IssueEquipmentModal.vue'
import EquipmentModal from '@/components/EquipmentModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const equipment = ref(null)
const rentalHistory = ref([])
const showIssueModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)

const isAdmin = computed(() => authStore.user?.role === 'Admin')
const canIssue = computed(() => equipment.value?.status === 'Available' && !isAdmin.value)

onMounted(async () => {
  await loadEquipment()
  if (isAdmin.value) {
    await loadRentalHistory()
  }
})

const loadEquipment = async () => {
  try {
    equipment.value = await equipmentService.getById(route.params.id)
  } catch (error) {
    console.error('Failed to load equipment:', error)
    router.push('/equipment')
  }
}

const loadRentalHistory = async () => {
  try {
    rentalHistory.value = await rentalService.getEquipmentHistory(route.params.id)
  } catch (error) {
    console.error('Failed to load rental history:', error)
  }
}

const getStatusClass = (status) => {
  const classes = {
    'Available': 'badge bg-success',
    'Rented': 'badge bg-warning',
    'Maintenance': 'badge bg-danger'
  }
  return classes[status] || 'badge bg-secondary'
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString()
}

const issueEquipment = () => {
  showIssueModal.value = true
}

const editEquipment = () => {
  showEditModal.value = true
}

const deleteEquipment = () => {
  showDeleteModal.value = true
}

const handleIssueSuccess = () => {
  showIssueModal.value = false
  loadEquipment()
}

const handleEditSave = async (equipmentData) => {
  try {
    await equipmentService.update(equipment.value.id, equipmentData)
    showEditModal.value = false
    loadEquipment()
  } catch (error) {
    console.error('Failed to update equipment:', error)
  }
}

const confirmDelete = async () => {
  try {
    await equipmentService.delete(equipment.value.id)
    showDeleteModal.value = false
    router.push('/equipment')
  } catch (error) {
    console.error('Failed to delete equipment:', error)
  }
}
</script>
```

---

## Phase 5: Customer Management

### Step 5.1: Create Customer Service
```javascript
// src/services/customer.js
import api from './api'

export const customerService = {
  async getAll() {
    const response = await api.get('/customers')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/customers/${id}`)
    return response.data
  },

  async create(customer) {
    const response = await api.post('/customers', customer)
    return response.data
  },

  async update(id, customer) {
    const response = await api.put(`/customers/${id}`, customer)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/customers/${id}`)
    return response.data
  },

  async getRentals(id) {
    const response = await api.get(`/customers/${id}/rentals`)
    return response.data
  },

  async getActiveRental(id) {
    const response = await api.get(`/customers/${id}/active-rental`)
    return response.data
  }
}
```

### Step 5.2: Create Customer List Component
```vue
<!-- src/views/Customers.vue -->
<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Customers</h1>
      <button @click="showCreateModal = true" class="btn btn-primary">
        Add Customer
      </button>
    </div>
    
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Name</th>
                <th>Username</th>
                <th>Role</th>
                <th>Active Rental</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customer in customers" :key="customer.id">
                <td>{{ customer.name }}</td>
                <td>{{ customer.username }}</td>
                <td>
                  <span :class="getRoleClass(customer.role)">
                    {{ customer.role }}
                  </span>
                </td>
                <td>
                  <span v-if="customer.activeRental" class="text-warning">
                    {{ customer.activeRental.equipmentName }}
                  </span>
                  <span v-else class="text-muted">None</span>
                </td>
                <td>
                  <div class="btn-group" role="group">
                    <router-link 
                      :to="`/customers/${customer.id}`" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </router-link>
                    <button 
                      @click="editCustomer(customer)" 
                      class="btn btn-sm btn-outline-secondary"
                    >
                      Edit
                    </button>
                    <button 
                      @click="deleteCustomer(customer)" 
                      class="btn btn-sm btn-outline-danger"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!-- Create/Edit Modal -->
    <CustomerModal 
      v-if="showCreateModal || showEditModal"
      :customer="editingCustomer"
      @close="closeModal"
      @save="handleSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Customer"
      message="Are you sure you want to delete this customer and their history?"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { customerService } from '@/services/customer'
import CustomerModal from '@/components/CustomerModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const customers = ref([])
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const editingCustomer = ref(null)
const deletingCustomer = ref(null)

onMounted(() => {
  loadCustomers()
})

const loadCustomers = async () => {
  try {
    customers.value = await customerService.getAll()
  } catch (error) {
    console.error('Failed to load customers:', error)
  }
}

const getRoleClass = (role) => {
  return role === 'Admin' ? 'badge bg-danger' : 'badge bg-primary'
}

const editCustomer = (customer) => {
  editingCustomer.value = customer
  showEditModal.value = true
}

const deleteCustomer = (customer) => {
  deletingCustomer.value = customer
  showDeleteModal.value = true
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingCustomer.value = null
}

const handleSave = async (customerData) => {
  try {
    if (editingCustomer.value) {
      await customerService.update(editingCustomer.value.id, customerData)
    } else {
      await customerService.create(customerData)
    }
    closeModal()
    loadCustomers()
  } catch (error) {
    console.error('Failed to save customer:', error)
  }
}

const confirmDelete = async () => {
  try {
    await customerService.delete(deletingCustomer.value.id)
    showDeleteModal.value = false
    deletingCustomer.value = null
    loadCustomers()
  } catch (error) {
    console.error('Failed to delete customer:', error)
  }
}
</script>
```

---

## Phase 6: Rental Management

### Step 6.1: Create Rental Service
```javascript
// src/services/rental.js
import api from './api'

export const rentalService = {
  async getAll() {
    const response = await api.get('/rentals')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/rentals/${id}`)
    return response.data
  },

  async getActive() {
    const response = await api.get('/rentals/active')
    return response.data
  },

  async getCompleted() {
    const response = await api.get('/rentals/completed')
    return response.data
  },

  async getOverdue() {
    const response = await api.get('/rentals/overdue')
    return response.data
  },

  async issue(rentalData) {
    const response = await api.post('/rentals/issue', rentalData)
    return response.data
  },

  async return(rentalData) {
    const response = await api.post('/rentals/return', rentalData)
    return response.data
  },

  async extend(id, extensionData) {
    const response = await api.put(`/rentals/${id}`, extensionData)
    return response.data
  },

  async cancel(id) {
    const response = await api.delete(`/rentals/${id}`)
    return response.data
  },

  async getEquipmentHistory(equipmentId) {
    const response = await api.get(`/rentals/equipment/${equipmentId}`)
    return response.data
  }
}
```

### Step 6.2: Create Rental List Component
```vue
<!-- src/views/Rentals.vue -->
<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Rentals</h1>
      <div>
        <button 
          @click="showIssueModal = true" 
          class="btn btn-primary me-2"
        >
          Issue Equipment
        </button>
        <button 
          @click="showReturnModal = true" 
          class="btn btn-success"
        >
          Return Equipment
        </button>
      </div>
    </div>
    
    <!-- Filter Tabs -->
    <ul class="nav nav-tabs mb-4">
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'all' }]"
          @click="setActiveTab('all')"
        >
          All Rentals
        </button>
      </li>
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'active' }]"
          @click="setActiveTab('active')"
        >
          Active
        </button>
      </li>
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'completed' }]"
          @click="setActiveTab('completed')"
        >
          Completed
        </button>
      </li>
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'overdue' }]"
          @click="setActiveTab('overdue')"
        >
          Overdue
        </button>
      </li>
    </ul>
    
    <!-- Rentals Table -->
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Equipment</th>
                <th>Customer</th>
                <th>Issue Date</th>
                <th>Due Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="rental in filteredRentals" 
                :key="rental.id"
                :class="{ 'table-danger': rental.status === 'Overdue' }"
              >
                <td>{{ rental.equipmentName }}</td>
                <td>{{ rental.customerName }}</td>
                <td>{{ formatDate(rental.issuedAt) }}</td>
                <td>{{ formatDate(rental.dueDate) }}</td>
                <td>
                  <span :class="getStatusClass(rental.status)">
                    {{ rental.status }}
                  </span>
                </td>
                <td>
                  <div class="btn-group" role="group">
                    <router-link 
                      :to="`/rentals/${rental.id}`" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </router-link>
                    <button 
                      v-if="rental.status === 'Active'"
                      @click="extendRental(rental)" 
                      class="btn btn-sm btn-outline-warning"
                    >
                      Extend
                    </button>
                    <button 
                      v-if="rental.status === 'Active'"
                      @click="returnRental(rental)" 
                      class="btn btn-sm btn-outline-success"
                    >
                      Return
                    </button>
                    <button 
                      v-if="isAdmin && rental.status === 'Overdue'"
                      @click="forceReturn(rental)" 
                      class="btn btn-sm btn-danger"
                    >
                      Force Return
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!-- Issue Equipment Modal -->
    <IssueEquipmentModal 
      v-if="showIssueModal"
      @close="showIssueModal = false"
      @success="handleIssueSuccess"
    />
    
    <!-- Return Equipment Modal -->
    <ReturnEquipmentModal 
      v-if="showReturnModal"
      @close="showReturnModal = false"
      @success="handleReturnSuccess"
    />
    
    <!-- Extend Rental Modal -->
    <ExtendRentalModal 
      v-if="showExtendModal"
      :rental="extendingRental"
      @close="showExtendModal = false"
      @success="handleExtendSuccess"
    />
    
    <!-- Force Return Modal -->
    <ConfirmationModal
      v-if="showForceReturnModal"
      title="Force Return Equipment"
      message="Are you sure you want to force return this equipment?"
      @confirm="confirmForceReturn"
      @cancel="showForceReturnModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { rentalService } from '@/services/rental'
import IssueEquipmentModal from '@/components/IssueEquipmentModal.vue'
import ReturnEquipmentModal from '@/components/ReturnEquipmentModal.vue'
import ExtendRentalModal from '@/components/ExtendRentalModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const activeTab = ref('all')
const allRentals = ref([])
const activeRentals = ref([])
const completedRentals = ref([])
const overdueRentals = ref([])

const showIssueModal = ref(false)
const showReturnModal = ref(false)
const showExtendModal = ref(false)
const showForceReturnModal = ref(false)
const extendingRental = ref(null)
const forceReturnRental = ref(null)

const filteredRentals = computed(() => {
  switch (activeTab.value) {
    case 'active': return activeRentals.value
    case 'completed': return completedRentals.value
    case 'overdue': return overdueRentals.value
    default: return allRentals.value
  }
})

onMounted(() => {
  loadRentals()
})

const loadRentals = async () => {
  try {
    const [all, active, completed, overdue] = await Promise.all([
      rentalService.getAll(),
      rentalService.getActive(),
      rentalService.getCompleted(),
      rentalService.getOverdue()
    ])
    
    allRentals.value = all
    activeRentals.value = active
    completedRentals.value = completed
    overdueRentals.value = overdue
  } catch (error) {
    console.error('Failed to load rentals:', error)
  }
}

const setActiveTab = (tab) => {
  activeTab.value = tab
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString()
}

const getStatusClass = (status) => {
  const classes = {
    'Active': 'badge bg-success',
    'Completed': 'badge bg-primary',
    'Overdue': 'badge bg-danger',
    'Cancelled': 'badge bg-secondary'
  }
  return classes[status] || 'badge bg-secondary'
}

const extendRental = (rental) => {
  extendingRental.value = rental
  showExtendModal.value = true
}

const returnRental = (rental) => {
  // Implementation for return rental
}

const forceReturn = (rental) => {
  forceReturnRental.value = rental
  showForceReturnModal.value = true
}

const handleIssueSuccess = () => {
  showIssueModal.value = false
  loadRentals()
}

const handleReturnSuccess = () => {
  showReturnModal.value = false
  loadRentals()
}

const handleExtendSuccess = () => {
  showExtendModal.value = false
  extendingRental.value = null
  loadRentals()
}

const confirmForceReturn = async () => {
  try {
    await rentalService.cancel(forceReturnRental.value.id)
    showForceReturnModal.value = false
    forceReturnRental.value = null
    loadRentals()
  } catch (error) {
    console.error('Failed to force return:', error)
  }
}
</script>
```

---

## Phase 7: Modal Components

### Step 7.1: Create Equipment Modal
```vue
<!-- src/components/EquipmentModal.vue -->
<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            {{ equipment ? 'Edit Equipment' : 'Add Equipment' }}
          </h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <div class="mb-3">
              <label for="name" class="form-label">Name</label>
              <input 
                type="text" 
                class="form-control" 
                id="name" 
                v-model="form.name"
                required
              />
            </div>
            
            <div class="mb-3">
              <label for="category" class="form-label">Category</label>
              <input 
                type="text" 
                class="form-control" 
                id="category" 
                v-model="form.category"
                required
              />
            </div>
            
            <div class="mb-3">
              <label for="condition" class="form-label">Condition</label>
              <select class="form-select" id="condition" v-model="form.condition" required>
                <option value="">Select Condition</option>
                <option value="Excellent">Excellent</option>
                <option value="Good">Good</option>
                <option value="Fair">Fair</option>
                <option value="Poor">Poor</option>
              </select>
            </div>
            
            <div class="mb-3">
              <label for="description" class="form-label">Description</label>
              <textarea 
                class="form-control" 
                id="description" 
                v-model="form.description"
                rows="3"
              ></textarea>
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ equipment ? 'Update' : 'Create' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  equipment: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['close', 'save'])

const form = ref({
  name: '',
  category: '',
  condition: '',
  description: ''
})

const loading = ref(false)

onMounted(() => {
  if (props.equipment) {
    form.value = { ...props.equipment }
  }
})

const handleSubmit = async () => {
  loading.value = true
  try {
    emit('save', form.value)
  } finally {
    loading.value = false
  }
}
</script>
```

### Step 7.2: Create Issue Equipment Modal
```vue
<!-- src/components/IssueEquipmentModal.vue -->
<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Issue Equipment</h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <div class="mb-3">
              <label for="equipment" class="form-label">Equipment</label>
              <select class="form-select" id="equipment" v-model="form.equipmentId" required>
                <option value="">Select Equipment</option>
                <option 
                  v-for="equipment in availableEquipment" 
                  :key="equipment.id" 
                  :value="equipment.id"
                >
                  {{ equipment.name }} - {{ equipment.category }}
                </option>
              </select>
            </div>
            
            <div v-if="isAdmin" class="mb-3">
              <label for="customer" class="form-label">Customer</label>
              <select class="form-select" id="customer" v-model="form.customerId" required>
                <option value="">Select Customer</option>
                <option 
                  v-for="customer in customers" 
                  :key="customer.id" 
                  :value="customer.id"
                >
                  {{ customer.name }} ({{ customer.username }})
                </option>
              </select>
            </div>
            
            <div class="mb-3">
              <label for="dueDate" class="form-label">Due Date</label>
              <input 
                type="date" 
                class="form-control" 
                id="dueDate" 
                v-model="form.dueDate"
                required
              />
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              Issue Equipment
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { equipmentService } from '@/services/equipment'
import { customerService } from '@/services/customer'
import { rentalService } from '@/services/rental'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const emit = defineEmits(['close', 'success'])

const form = ref({
  equipmentId: props.equipment?.id || '',
  customerId: isAdmin.value ? '' : authStore.user?.id,
  dueDate: ''
})

const availableEquipment = ref([])
const customers = ref([])
const loading = ref(false)

onMounted(async () => {
  await loadAvailableEquipment()
  if (isAdmin.value) {
    await loadCustomers()
  }
})

const loadAvailableEquipment = async () => {
  try {
    availableEquipment.value = await equipmentService.getAvailable()
  } catch (error) {
    console.error('Failed to load available equipment:', error)
  }
}

const loadCustomers = async () => {
  try {
    customers.value = await customerService.getAll()
  } catch (error) {
    console.error('Failed to load customers:', error)
  }
}

const handleSubmit = async () => {
  loading.value = true
  try {
    await rentalService.issue(form.value)
    emit('success')
  } catch (error) {
    console.error('Failed to issue equipment:', error)
  } finally {
    loading.value = false
  }
}
</script>
```

### Step 7.3: Create Confirmation Modal
```vue
<!-- src/components/ConfirmationModal.vue -->
<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">{{ title }}</h5>
          <button type="button" class="btn-close" @click="$emit('cancel')"></button>
        </div>
        
        <div class="modal-body">
          <p>{{ message }}</p>
        </div>
        
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="$emit('cancel')">
            Cancel
          </button>
          <button type="button" class="btn btn-danger" @click="$emit('confirm')">
            Confirm
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  message: {
    type: String,
    required: true
  }
})

defineEmits(['confirm', 'cancel'])
</script>
```

---

## Phase 8: Utility Functions

### Step 8.1: Create Date Utility
```javascript
// src/utils/date.js
export const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString()
}

export const formatDateTime = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString()
}

export const getDaysDifference = (startDate, endDate) => {
  const start = new Date(startDate)
  const end = new Date(endDate)
  const diffTime = Math.abs(end - start)
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}
```

### Step 8.2: Create Toast Notification Component
```vue
<!-- src/components/Toast.vue -->
<template>
  <div class="toast-container position-fixed top-0 end-0 p-3">
    <div 
      v-for="toast in toasts" 
      :key="toast.id"
      :class="['toast', `bg-${toast.type}`]"
      role="alert"
    >
      <div class="toast-header">
        <strong class="me-auto">{{ toast.title }}</strong>
        <button 
          type="button" 
          class="btn-close" 
          @click="removeToast(toast.id)"
        ></button>
      </div>
      <div class="toast-body">
        {{ toast.message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const toasts = ref([])

const addToast = (title, message, type = 'success') => {
  const id = Date.now()
  toasts.value.push({ id, title, message, type })
  
  setTimeout(() => {
    removeToast(id)
  }, 5000)
}

const removeToast = (id) => {
  const index = toasts.value.findIndex(toast => toast.id === id)
  if (index > -1) {
    toasts.value.splice(index, 1)
  }
}

defineExpose({ addToast })
</script>
```

---

## Phase 9: Final Integration

### Step 9.1: Update Main App Component
```vue
<!-- src/App.vue -->
<template>
  <div id="app">
    <Layout v-if="isAuthenticated" />
    <router-view v-else />
    <Toast ref="toast" />
  </div>
</template>

<script setup>
import { computed, provide } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Layout from '@/components/Layout.vue'
import Toast from '@/components/Toast.vue'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)

// Provide toast functionality to all components
const toast = ref(null)
provide('toast', toast)
</script>
```

### Step 9.2: Add Global Styles
```css
/* src/assets/main.css */
.bg-primary { background-color: #0d6efd !important; }
.bg-success { background-color: #198754 !important; }
.bg-warning { background-color: #ffc107 !important; }
.bg-danger { background-color: #dc3545 !important; }

.table-danger {
  background-color: rgba(220, 53, 69, 0.1) !important;
}

.spinner-border-sm {
  width: 1rem;
  height: 1rem;
}

.modal.show {
  background-color: rgba(0, 0, 0, 0.5);
}
```

### Step 9.3: Create Environment Configuration
```javascript
// src/config/index.js
export const config = {
  apiBaseUrl: 'http://localhost:5129/api',
  appName: 'Equipment Rental Management System'
}
```

---

## Phase 10: Testing & Deployment

### Step 10.1: Test All Features
1. **Authentication Flow**
   - Login with valid credentials
   - Logout functionality
   - Route protection

2. **Dashboard Functionality**
   - Load statistics correctly
   - Quick actions work
   - Role-based display

3. **Equipment Management**
   - CRUD operations
   - Status updates
   - Rental history

4. **Customer Management**
   - User profile updates
   - Admin customer management
   - Role-based access

5. **Rental Management**
   - Issue equipment
   - Return equipment
   - Extend rentals
   - View rental history

### Step 10.2: Error Handling
- Test API error responses
- Verify error messages display correctly
- Test network failure scenarios

### Step 10.3: Responsive Design
- Test on different screen sizes
- Ensure mobile compatibility
- Verify all modals work on mobile

---

## Development Notes

### Key Implementation Points:
1. **Authentication**: JWT tokens stored in localStorage
2. **Role-based Access**: Admin vs User permissions
3. **API Integration**: Axios with interceptors
4. **State Management**: Pinia for global state
5. **Routing**: Vue Router with navigation guards
6. **UI Framework**: Bootstrap for styling
7. **Date Formatting**: Simple date display (no time)
8. **Error Handling**: HTTP status code based responses
9. **No Client Validation**: Backend handles all validation
10. **No Real-time Updates**: Static data loading

### File Structure:
```
src/
├── components/
│   ├── Layout.vue
│   ├── EquipmentModal.vue
│   ├── IssueEquipmentModal.vue
│   ├── ReturnEquipmentModal.vue
│   ├── ExtendRentalModal.vue
│   ├── CustomerModal.vue
│   ├── ConfirmationModal.vue
│   └── Toast.vue
├── views/
│   ├── Login.vue
│   ├── Dashboard.vue
│   ├── Equipment.vue
│   ├── EquipmentDetails.vue
│   ├── Customers.vue
│   ├── CustomerDetails.vue
│   ├── Rentals.vue
│   └── RentalDetails.vue
├── services/
│   ├── api.js
│   ├── equipment.js
│   ├── customer.js
│   └── rental.js
├── stores/
│   └── auth.js
├── utils/
│   └── date.js
├── router/
│   └── index.js
└── assets/
    └── main.css
```

This implementation guide provides a complete roadmap for building the Vue frontend. Follow each step sequentially, and you'll have a fully functional equipment rental management system frontend that integrates seamlessly with your existing .NET backend.
