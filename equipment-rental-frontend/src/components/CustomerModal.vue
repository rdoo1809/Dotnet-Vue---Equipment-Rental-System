<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            {{ customer ? 'Edit Customer' : 'Add Customer' }}
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
              <label for="username" class="form-label">Username</label>
              <input 
                type="text" 
                class="form-control" 
                id="username" 
                v-model="form.username"
                required
              />
            </div>
            
            <div class="mb-3">
              <label for="password" class="form-label">
                {{ customer ? 'New Password (leave blank to keep current)' : 'Password' }}
              </label>
              <input 
                type="password" 
                class="form-control" 
                id="password" 
                v-model="form.password"
                :required="!customer"
              />
            </div>
            
            <div class="mb-3">
              <label for="role" class="form-label">Role</label>
              <select class="form-select" id="role" v-model="form.role" required>
                <option value="">Select Role</option>
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ customer ? 'Update' : 'Create' }}
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
  customer: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['close', 'save'])

const form = ref({
  name: '',
  username: '',
  password: '',
  role: 'User'
})

const loading = ref(false)

onMounted(() => {
  if (props.customer) {
    form.value = { 
      name: props.customer.name,
      username: props.customer.username,
      password: '',
      role: props.customer.role
    }
  }
})

const handleSubmit = async () => {
  loading.value = true
  try {
    // Remove password if empty for updates
    const submitData = { ...form.value }
    if (props.customer && !submitData.password) {
      delete submitData.password
    }
    emit('save', submitData)
  } finally {
    loading.value = false
  }
}
</script>
