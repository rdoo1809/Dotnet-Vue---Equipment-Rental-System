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
