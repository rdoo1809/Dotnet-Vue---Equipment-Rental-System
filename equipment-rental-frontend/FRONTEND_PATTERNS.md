# Frontend Patterns & Implementation Guide

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Component Patterns](#component-patterns)
3. [State Management](#state-management)
4. [API Integration](#api-integration)
5. [Routing & Navigation](#routing--navigation)
6. [UI/UX Patterns](#uiux-patterns)
7. [Authentication & Security](#authentication--security)
8. [Error Handling](#error-handling)
9. [Performance Optimization](#performance-optimization)
10. [Testing Patterns](#testing-patterns)

## Architecture Overview

### Technology Stack
- **Vue.js 3** with Composition API
- **Vite** for build tooling
- **Vue Router 4** for routing
- **Pinia** for state management
- **Axios** for HTTP requests
- **Bootstrap 5** for UI components

### Project Structure
```
src/
├── components/     # Reusable UI components
├── views/         # Page-level components
├── services/      # API service layer
├── stores/        # Pinia state management
├── router/        # Vue Router configuration
├── utils/         # Utility functions
└── assets/        # Static assets
```

## Component Patterns

### 1. Basic Component Structure

```vue
<template>
  <div class="component-container">
    <!-- Template content -->
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

// Props and emits
const props = defineProps({
  // prop definitions
})

const emit = defineEmits(['event-name'])

// Reactive state
const localState = ref('')
const authStore = useAuthStore()

// Computed properties
const computedValue = computed(() => {
  // computation logic
})

// Lifecycle hooks
onMounted(() => {
  // initialization logic
})

// Methods
const handleAction = () => {
  // action logic
}
</script>

<style scoped>
/* Component-specific styles */
</style>
```

### 2. Modal Component Pattern

```vue
<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">{{ title }}</h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <!-- Form content -->
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ submitText }}
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
  item: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['close', 'save'])

const form = ref({
  // form fields
})

const loading = ref(false)

onMounted(() => {
  if (props.item) {
    form.value = { ...props.item }
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

### 3. List/Table Component Pattern

```vue
<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>{{ title }}</h1>
      <button 
        v-if="canCreate" 
        @click="showCreateModal = true" 
        class="btn btn-primary"
      >
        Add {{ itemName }}
      </button>
    </div>
    
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th v-for="column in columns" :key="column.key">
                  {{ column.label }}
                </th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in items" :key="item.id">
                <td v-for="column in columns" :key="column.key">
                  {{ getColumnValue(item, column.key) }}
                </td>
                <td>
                  <div class="btn-group" role="group">
                    <button 
                      @click="viewItem(item)" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </button>
                    <button 
                      @click="editItem(item)" 
                      class="btn btn-sm btn-outline-secondary"
                    >
                      Edit
                    </button>
                    <button 
                      @click="deleteItem(item)" 
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
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const canCreate = computed(() => authStore.user?.role === 'Admin')

const items = ref([])
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const editingItem = ref(null)
const deletingItem = ref(null)

onMounted(() => {
  loadItems()
})

const loadItems = async () => {
  try {
    // API call to load items
  } catch (error) {
    console.error('Failed to load items:', error)
  }
}

const getColumnValue = (item, key) => {
  return item[key]
}

const viewItem = (item) => {
  // Navigate to item details
}

const editItem = (item) => {
  editingItem.value = item
  showEditModal.value = true
}

const deleteItem = (item) => {
  deletingItem.value = item
  showDeleteModal.value = true
}
</script>
```

## State Management

### 1. Pinia Store Pattern

```javascript
// stores/feature.js
import { defineStore } from 'pinia'
import api from '@/services/api'

export const useFeatureStore = defineStore('feature', {
  state: () => ({
    items: [],
    loading: false,
    error: null
  }),

  getters: {
    getItemById: (state) => (id) => {
      return state.items.find(item => item.id === id)
    },
    
    filteredItems: (state) => (filter) => {
      return state.items.filter(item => 
        item.name.toLowerCase().includes(filter.toLowerCase())
      )
    }
  },

  actions: {
    async fetchItems() {
      this.loading = true
      this.error = null
      try {
        const response = await api.get('/items')
        this.items = response.data
      } catch (error) {
        this.error = error.message
        throw error
      } finally {
        this.loading = false
      }
    },

    async createItem(itemData) {
      try {
        const response = await api.post('/items', itemData)
        this.items.push(response.data)
        return response.data
      } catch (error) {
        this.error = error.message
        throw error
      }
    },

    async updateItem(id, itemData) {
      try {
        const response = await api.put(`/items/${id}`, itemData)
        const index = this.items.findIndex(item => item.id === id)
        if (index !== -1) {
          this.items[index] = response.data
        }
        return response.data
      } catch (error) {
        this.error = error.message
        throw error
      }
    },

    async deleteItem(id) {
      try {
        await api.delete(`/items/${id}`)
        this.items = this.items.filter(item => item.id !== id)
      } catch (error) {
        this.error = error.message
        throw error
      }
    }
  }
})
```

### 2. Authentication Store Pattern

```javascript
// stores/auth.js
import { defineStore } from 'pinia'
import api from '@/services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token'),
    isAuthenticated: !!localStorage.getItem('token')
  }),

  getters: {
    isAdmin: (state) => state.user?.role === 'Admin',
    userName: (state) => state.user?.name || '',
    userRole: (state) => state.user?.role || ''
  },

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
        const response = await api.get('/user/me')
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

## API Integration

### 1. API Service Pattern

```javascript
// services/api.js
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

### 2. Feature Service Pattern

```javascript
// services/equipment.js
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

### 3. Component API Usage Pattern

```vue
<script setup>
import { ref, onMounted } from 'vue'
import { equipmentService } from '@/services/equipment'

const equipmentList = ref([])
const loading = ref(false)
const error = ref('')

const loadEquipment = async () => {
  loading.value = true
  error.value = ''
  try {
    equipmentList.value = await equipmentService.getAll()
  } catch (err) {
    error.value = err.message
    console.error('Failed to load equipment:', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadEquipment()
})
</script>
```

## Routing & Navigation

### 1. Route Configuration Pattern

```javascript
// router/index.js
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
    path: '/customer',
    name: 'Customers',
    component: () => import('@/views/Customers.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
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

### 2. Navigation Pattern

```vue
<script setup>
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const navigateToItem = (id) => {
  router.push(`/equipment/${id}`)
}

const goBack = () => {
  router.back()
}

const navigateWithQuery = () => {
  router.push({
    name: 'Equipment',
    query: { filter: 'available' }
  })
}
</script>
```

## UI/UX Patterns

### 1. Status Indicators Pattern

```vue
<template>
  <span :class="getStatusClass(status)">
    {{ status }}
  </span>
</template>

<script setup>
const props = defineProps({
  status: {
    type: String,
    required: true
  }
})

const getStatusClass = (status) => {
  const classes = {
    'Available': 'badge bg-success',
    'Rented': 'badge bg-warning',
    'Overdue': 'badge bg-danger',
    'Completed': 'badge bg-primary',
    'Maintenance': 'badge bg-secondary'
  }
  return classes[status] || 'badge bg-secondary'
}
</script>
```

### 2. Loading States Pattern

```vue
<template>
  <div>
    <div v-if="loading" class="text-center">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    
    <div v-else-if="error" class="alert alert-danger">
      {{ error }}
    </div>
    
    <div v-else>
      <!-- Content -->
    </div>
  </div>
</template>

<script setup>
const loading = ref(false)
const error = ref('')
</script>
```

### 3. Toast Notifications Pattern

```vue
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

## Authentication & Security

### 1. Route Guards Pattern

```javascript
// router/index.js
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  
  // Check authentication
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  // Check guest routes
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next('/dashboard')
    return
  }
  
  // Check admin routes
  if (to.meta.requiresAdmin && authStore.user?.role !== 'Admin') {
    next('/dashboard')
    return
  }
  
  next()
})
```

### 2. Component Authorization Pattern

```vue
<template>
  <div>
    <button 
      v-if="isAdmin" 
      @click="adminAction" 
      class="btn btn-danger"
    >
      Admin Action
    </button>
    
    <div v-if="canEdit" class="edit-controls">
      <!-- Edit controls -->
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')
const canEdit = computed(() => {
  return isAdmin.value || props.item?.userId === authStore.user?.id
})
</script>
```

## Error Handling

### 1. API Error Handling Pattern

```vue
<script setup>
import { ref } from 'vue'
import { equipmentService } from '@/services/equipment'

const equipmentList = ref([])
const loading = ref(false)
const error = ref('')

const loadEquipment = async () => {
  loading.value = true
  error.value = ''
  try {
    equipmentList.value = await equipmentService.getAll()
  } catch (err) {
    error.value = err.response?.data?.message || 'Failed to load equipment'
    console.error('API Error:', err)
  } finally {
    loading.value = false
  }
}
</script>
```

### 2. Form Validation Pattern

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <div class="mb-3">
      <label for="name" class="form-label">Name</label>
      <input 
        type="text" 
        class="form-control"
        :class="{ 'is-invalid': errors.name }"
        id="name" 
        v-model="form.name"
        required
      />
      <div v-if="errors.name" class="invalid-feedback">
        {{ errors.name }}
      </div>
    </div>
    
    <button type="submit" class="btn btn-primary" :disabled="loading">
      <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
      Submit
    </button>
  </form>
</template>

<script setup>
const form = ref({
  name: '',
  email: ''
})

const errors = ref({})
const loading = ref(false)

const validateForm = () => {
  errors.value = {}
  
  if (!form.value.name) {
    errors.value.name = 'Name is required'
  }
  
  if (!form.value.email) {
    errors.value.email = 'Email is required'
  } else if (!/\S+@\S+\.\S+/.test(form.value.email)) {
    errors.value.email = 'Email is invalid'
  }
  
  return Object.keys(errors.value).length === 0
}

const handleSubmit = async () => {
  if (!validateForm()) return
  
  loading.value = true
  try {
    await equipmentService.create(form.value)
    // Handle success
  } catch (error) {
    // Handle error
  } finally {
    loading.value = false
  }
}
</script>
```

## Performance Optimization

### 1. Lazy Loading Pattern

```javascript
// router/index.js
const routes = [
  {
    path: '/equipment',
    name: 'Equipment',
    component: () => import('@/views/Equipment.vue'),
    meta: { requiresAuth: true }
  }
]
```

### 2. Computed Properties Pattern

```vue
<script setup>
import { ref, computed } from 'vue'

const items = ref([])
const filter = ref('')

const filteredItems = computed(() => {
  if (!filter.value) return items.value
  
  return items.value.filter(item => 
    item.name.toLowerCase().includes(filter.value.toLowerCase())
  )
})

const itemCount = computed(() => filteredItems.value.length)
</script>
```

### 3. Event Handling Optimization

```vue
<template>
  <div>
    <button 
      v-for="item in items" 
      :key="item.id"
      @click="handleItemClick(item)"
      class="btn btn-primary"
    >
      {{ item.name }}
    </button>
  </div>
</template>

<script setup>
const handleItemClick = (item) => {
  // Handle click with item data
  console.log('Clicked item:', item)
}
</script>
```

## Testing Patterns

### 1. Component Testing Pattern

```javascript
// tests/components/EquipmentModal.test.js
import { mount } from '@vue/test-utils'
import EquipmentModal from '@/components/EquipmentModal.vue'

describe('EquipmentModal', () => {
  it('renders create form when no equipment provided', () => {
    const wrapper = mount(EquipmentModal)
    expect(wrapper.find('h5').text()).toBe('Add Equipment')
  })

  it('renders edit form when equipment provided', () => {
    const equipment = { id: 1, name: 'Test Equipment' }
    const wrapper = mount(EquipmentModal, {
      props: { equipment }
    })
    expect(wrapper.find('h5').text()).toBe('Edit Equipment')
  })

  it('emits save event on form submit', async () => {
    const wrapper = mount(EquipmentModal)
    await wrapper.find('form').trigger('submit')
    expect(wrapper.emitted('save')).toBeTruthy()
  })
})
```

### 2. Store Testing Pattern

```javascript
// tests/stores/auth.test.js
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('initializes with no user', () => {
    const store = useAuthStore()
    expect(store.user).toBeNull()
    expect(store.isAuthenticated).toBe(false)
  })

  it('sets user on login', async () => {
    const store = useAuthStore()
    const credentials = { username: 'test', password: 'test' }
    
    // Mock API response
    const mockResponse = {
      data: { token: 'test-token', user: { id: 1, name: 'Test User' } }
    }
    
    // Test login
    const result = await store.login(credentials)
    expect(result.success).toBe(true)
    expect(store.isAuthenticated).toBe(true)
  })
})
```

## Common Utilities

### 1. Date Formatting Utility

```javascript
// utils/date.js
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

### 2. API Error Handling Utility

```javascript
// utils/error.js
export const handleApiError = (error) => {
  if (error.response) {
    // Server responded with error status
    return error.response.data?.message || 'Server error occurred'
  } else if (error.request) {
    // Request was made but no response received
    return 'Network error - please check your connection'
  } else {
    // Something else happened
    return 'An unexpected error occurred'
  }
}
```

This comprehensive guide provides patterns and examples for implementing features in the Equipment Rental Frontend application. Use these patterns as reference when planning or implementing new features to maintain consistency with the existing codebase.
